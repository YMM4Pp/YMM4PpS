using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace NumberText
{
    internal class NumberText : IShapePlugin
    {
        public string Name => "数値テキスト";
        public bool IsExoShapeSupported => false;
        public bool IsExoMaskSupported => false;
        public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
        {
            return new NumberTextParamater(sharedData);
        }
    }
}
