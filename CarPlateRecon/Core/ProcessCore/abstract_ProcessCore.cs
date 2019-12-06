using OpenCvSharp;


namespace CarPlateRecon.Core.ProcessCore
{
    public abstract class abstract_ProcessCore
    {
        public abstract void SetImage(byte[] OriginalSource);
        public abstract Mat ImageProcess();
    }
}
