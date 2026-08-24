
namespace EToken.Domain.Services;


public interface ITotpService
{
string GenerateCode(byte[] secret, DateTimeOffset time);
bool Verify(byte[] secret, string submittedCode, DateTimeOffset serverTime, int driftSteps = 1);
}




public class TotpService : ITotpService
{
private const int StepSeconds = 30;
public string GenerateCode(byte[] secret, DateTimeOffset time)
{
var totp = new OtpNet.Totp(secret, step: StepSeconds, totpSize: 6);
return totp.ComputeTotp(time.UtcDateTime);
}
public bool Verify(byte[] secret, string submittedCode, DateTimeOffset serverTime, int driftSteps = 1)
{
var totp = new OtpNet.Totp(secret, step: StepSeconds, totpSize: 6);
return totp.VerifyTotp(
serverTime.UtcDateTime,
submittedCode,out _,
new OtpNet.VerificationWindow(previous: driftSteps, future: driftSteps)
);
}
}