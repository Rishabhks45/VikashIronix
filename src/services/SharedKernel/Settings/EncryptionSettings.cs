namespace SharedKernel.Settings;

public class EncryptionSettings
{
    public string MasterKey { get; set; } = string.Empty;

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(MasterKey) && MasterKey.Length >= 32;
    }
}