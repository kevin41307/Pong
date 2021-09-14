using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class RaycastExample : MonoBehaviour
{

    private void Start()
    {
        RaycasExample();
    }

    private void RaycasExample()
    {
        // Perform a single raycast using RaycastCommand and wait for it to complete
        // Setup the command and result buffers
        var results = new NativeArray<RaycastHit>(1, Allocator.Temp);

        var commands = new NativeArray<RaycastCommand>(1, Allocator.Temp);

        // Set the data of the first command
        Vector3 origin = Vector3.forward * -10;

        Vector3 direction = Vector3.forward;

        commands[0] = new RaycastCommand(origin, direction);

        // Schedule the batch of raycasts
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));

        // Wait for the batch processing job to complete
        handle.Complete();

        // Copy the result. If batchedHit.collider is null there was no hit
        RaycastHit batchedHit = results[0];


        // Dispose the buffers
        results.Dispose();
        commands.Dispose();
    }
}
