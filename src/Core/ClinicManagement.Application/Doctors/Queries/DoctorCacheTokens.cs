using Microsoft.Extensions.Primitives;

namespace ClinicManagement.Application.Doctors.Queries;

public static class DoctorCacheTokens
{
    private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

    public static IChangeToken GetChangeToken() => new CancellationChangeToken(_resetCacheToken.Token);

    public static void Clear()
    {
        _resetCacheToken.Cancel();
        _resetCacheToken.Dispose();
        _resetCacheToken = new CancellationTokenSource();
    }
}
