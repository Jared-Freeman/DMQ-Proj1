using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

//Threading using C# requires more upkeep than a simple Coroutine workflow seen normally in Unity.
// This MonoBehavior uses nested async Tasks with a CancellationToken generated from a CancellationTokenSource, allowing cancellation to be invoked in Start() after 1000 ms
public class FR_AsyncTaskExample : MonoBehaviour
{
    private void Start()
    {
        var cts = new System.Threading.CancellationTokenSource();
        cts.CancelAfter(1000);
        var T1 = ExampleTask(cts.Token);
        Debug.Log("start");
    }

    public async Task ExampleTask(System.Threading.CancellationToken ct)
    {
        Debug.Log("Init ExampleTask noTry");
        //System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource();
        System.Threading.CancellationTokenSource cts = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(ct);

        try
        {
            Debug.Log("Init ExampleTask");
            var Linkedct = cts.Token;

            await InfiniteLoop(Linkedct);
        }
        catch (System.OperationCanceledException)
        {

            Debug.Log("ExampleTask Canceled via timeout!");
        }
        finally
        {
            Debug.Log("Disposal of Cancellation Token Source!");
            cts.Dispose();
        }
    }

    async Task InfiniteLoop(System.Threading.CancellationToken ct)
    {
        System.Threading.CancellationTokenSource cts = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(ct);
        try
        {
            float timestart = Time.time;
            while (!ct.IsCancellationRequested)
            {
                Debug.Log("test");
                await Task.Yield();
            }
            Debug.Log("Routine Duration: " + (Time.time - timestart));
        }
        catch (System.OperationCanceledException)
        {

            Debug.Log("aaa");
        }
        finally
        {
            Debug.Log("AA!");
            cts.Dispose();
        }
    }
}
