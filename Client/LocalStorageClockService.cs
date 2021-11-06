using Blazored.LocalStorage;
using BlazorWorldClock.Shared;

namespace BlazorWorldClock.Client;

public class LocalStorageClockService : IClockService
{
    private readonly ILocalStorageService LocalStorage;

    private const string LocalStorageKey = "clocks";

    public LocalStorageClockService(ILocalStorageService localStorage)
    {
        this.LocalStorage = localStorage;
    }

    public async ValueTask<IEnumerable<Clock>> GetClocksAsync()
    {
        return await this.LocalStorage.GetItemAsync<Clock[]>(LocalStorageKey) ?? Enumerable.Empty<Clock>();
    }

    public async ValueTask AddClockAsync(Clock clock)
    {
        var clocks = await this.GetClocksAsync();
        clocks = clocks.Append(clock);
        await this.LocalStorage.SetItemAsync(LocalStorageKey, clocks);
    }

    public async ValueTask DeleteClockAsync(Guid id)
    {
        var clocks = await this.GetClocksAsync();
        clocks = clocks.Where(clock => clock.Id != id);
        await this.LocalStorage.SetItemAsync(LocalStorageKey, clocks);
    }

    public async ValueTask<Clock?> GetClockAsync(Guid id)
    {
        var clocks = await this.GetClocksAsync();
        return clocks.FirstOrDefault(clock => clock.Id == id);
    }

    public async ValueTask UpdateClockAsync(Guid id, Clock clock)
    {
        var clocks = await this.GetClocksAsync();
        var updateTo = clocks.FirstOrDefault(clock => clock.Id == id);
        if(updateTo == null) throw new Exception($"The clock was not found.");

        updateTo.Name = clock.Name;
        updateTo.TimeZoneId = clock.TimeZoneId;

        await this.LocalStorage.SetItemAsync(LocalStorageKey, clocks);
    }
}
