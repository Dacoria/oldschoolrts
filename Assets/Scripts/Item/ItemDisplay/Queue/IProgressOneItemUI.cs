using System;

public interface IProcesOneItemUI
{
    UIItemProcessing GetCurrentItemProcessed();
    float GetBuildTimeInSeconds(Enum type);
    bool ItemIsBeingProcessed => GetCurrentItemProcessed() != null;
}