using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class SceneObjectPawnPlayer
    : SceneObjectPawn,
    IStorageInventoryComponent,
    IStorageEquipmentComponent,
    IItemToolComponent
{
    private readonly SpawnerService _spawnerService;
    private readonly CameraService _cameraService;
    private readonly UnityEventsService _unityEventsService;
    private readonly RaycasterService _raycasterService;

    public Storage InventoryStorage { get; } = new(StorageTypes.StorageType.Inventory);

    public Storage EquipmentStorage { get; } = new(StorageTypes.StorageType.Equipment);
    public ObservableValue<int> EquipmentStorageSelectedIndex { get; } = new();
    private IDisposable _onEquipmentStorageSelectedIndexChangedSubscription;

    [JsonIgnore]
    public SceneObjectItemTool ItemTool => Children.ByType<SceneObjectItemTool>().FirstOrDefault();

    public Vector2 Move = Vector2.zero;
    public Vector2 Look = Vector2.zero;
    public bool Sprint;
    public bool Jump;

    private CharacterController _characterController;

    private readonly float walkingSpeed = 7.5f;
    private readonly float runningSpeed = 11.5f;
    private readonly float jumpSpeed = 8.0f;
    private readonly float gravity = 20.0f;
    private readonly float lookSpeed = 0.2f;
    private readonly float lookXLimit = 90.0f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float rotationY = 0;

    private readonly bool canMove = true;

    public SceneObjectPawnPlayer(
        SpawnerService spawnerService,
        CameraService cameraService,
        UnityEventsService unityEventsService,
        RaycasterService raycasterService)
    {
        _spawnerService = spawnerService;
        _cameraService = cameraService;
        _unityEventsService = unityEventsService;
        _raycasterService = raycasterService;
    }

    protected override void OnSpawn()
    {
        _characterController = GetBehaviour<CharacterController>();

        EquipmentStorage.ObservableItems.CollectionChanged += OnEquipmentCollectionChanged;

        _onEquipmentStorageSelectedIndexChangedSubscription = EquipmentStorageSelectedIndex.Subscribe(
            onInit: value => OnEquipmentSelectedIndexInit(value),
            onChange: (oldValue, newValue) => OnEquipmentSelectedIndexChanged(oldValue, newValue),
            onDeinit: value => OnEquipmentSelectedIndexDeinit(value));

        _unityEventsService.Events.OnUpdate += OnUpdate;

        base.OnSpawn();
    }

    protected override void OnDespawn()
    {
        EquipmentStorage.ObservableItems.CollectionChanged -= OnEquipmentCollectionChanged;

        _onEquipmentStorageSelectedIndexChangedSubscription.Dispose();
        _onEquipmentStorageSelectedIndexChangedSubscription = null;

        _unityEventsService.Events.OnUpdate -= OnUpdate;

        base.OnDespawn();
    }

    private void OnUpdate(float deltaTime)
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = Transform.TransformDirection(Vector3.forward);
        Vector3 right = Transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        float curSpeedX = canMove ? (Sprint ? runningSpeed : walkingSpeed) * Move.y : 0;
        float curSpeedY = canMove ? (Sprint ? runningSpeed : walkingSpeed) * Move.x : 0;

        float movementDirectionY = moveDirection.y;

        moveDirection = forward * curSpeedX + right * curSpeedY;

        if (Jump && canMove && _characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!_characterController.isGrounded)
        {
            moveDirection.y -= gravity * deltaTime;
        }

        // Move the controller
        _characterController.Move(moveDirection * deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            var rotationValue = Look;

            rotationY = rotationValue.x * lookSpeed;
            Transform.rotation *= Quaternion.Euler(0, rotationY, 0);

            rotationX -= rotationValue.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        }

        _cameraService.Camera.transform.SetPositionAndRotation(
            GetPositionForCamera(),
            GetRotationForCamera());

        ItemTool?.ItemToolService?.OnUpdate(this, deltaTime, _raycasterService.RayAllFromEyes(_raycasterService.DefaultItemsTakeLength));
    }

    private Vector3 GetPositionForCamera()
    {
        return Transform.position + new Vector3(0, 1.0f, 0);
    }

    private Quaternion GetRotationForCamera()
    {
        return Quaternion.Euler(rotationX, Transform.rotation.eulerAngles.y, 0);
    }

    private void OnEquipmentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnEquipmentSelectedIndexInit(EquipmentStorageSelectedIndex.Value);
    }

    private void OnEquipmentSelectedIndexInit(int index)
    {
        var itemKey = EquipmentStorage.ObservableItems.ElementAt(index);
        var itemInfo = Items.List[(int)itemKey];

        if (itemInfo.AdditionalInfo is ItemInfoTool itemInfoTool)
        {
            if (ItemTool is null || itemInfoTool.ItemToolKey != ItemTool.ItemToolKey)
            {
                ItemTool?.Dispose();

                _spawnerService.SpawnItemTool(itemInfoTool.ItemToolKey, this);
            }
        }
    }

    private void OnEquipmentSelectedIndexDeinit(int index)
    {
    }

    private void OnEquipmentSelectedIndexChanged(int oldIndex, int newIndex)
    {
        OnEquipmentSelectedIndexInit(newIndex);
    }
}
