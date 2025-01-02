using System;

public interface IProcesOneItemUI
{
    TypeProcessing GetCurrentItemProcessed();
    float GetBuildTimeInSeconds();
    bool ItemIsBeingProcessed => GetCurrentItemProcessed() != null;
}