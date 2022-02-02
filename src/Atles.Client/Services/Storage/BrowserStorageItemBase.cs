using System;

namespace Atles.Client.Services.Storage;

public abstract class BrowserStorageItemBase : IBrowserStorageItem
{
    public string Key { get; set; } = Guid.NewGuid().ToString();
}