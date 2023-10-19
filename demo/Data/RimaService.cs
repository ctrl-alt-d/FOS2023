using System.Collections.ObjectModel;

namespace demo.Data;

public class RimaService: ObservableCollection<Rima>
{
    private readonly SemaphoreSlim Semaphore = new (1);
    private readonly JutgeService JutgeService;
    public RimaService(JutgeService jutgeService)    
    {
        JutgeService = jutgeService;
    }

    public async Task AddRimaAsync(Rima rima)
    {
        await Semaphore.WaitAsync();
        await JutgeService.ValoraRima(rima);
        Add(rima);
        Semaphore.Release();
    }

}