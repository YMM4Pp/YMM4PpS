using System.ComponentModel.DataAnnotations;
using System.Windows.Ink;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace NumberText
{
    internal class NumberTextParamater : ShapeParameterBase
    {
        [Display(Name = "値", Description = "表示する数値")]
        [AnimationSlider("F4", "", -100, 100)]
        public Animation Number { get; } = new Animation(0);

        [Display(Name = "小数桁数", Description = "数値の小数点以下の桁数")]
        [AnimationSlider("F0", "", 0, 8)]
        public Animation DecimalPlaces { get; } = new Animation(0);

        [Display(Name = "3桁区切り", Description = "整数部を3桁ごとにカンマで区切ります。")]
        [ToggleSlider]
        public bool Sepalate { get => sepalate; set => Set(ref sepalate, value); }
        bool sepalate = false;

        [Display(Name = "フォント", Description = "フォント")]
        [FontComboBox]
        public string Font { get => font; set => Set(ref font, value); }
        string font = "メイリオ";

        [Display(Name = "サイズ", Description = "フォントのサイズ")]
        [AnimationSlider("F0", "px", 1, 100)]
        public Animation FontSize { get; } = new Animation(32);

        [Display(Name = "色", Description = "文字色")]
        [ColorPicker]
        public System.Windows.Media.Color Color { get => color; set => Set(ref color, value); }
        System.Windows.Media.Color color = System.Windows.Media.Colors.White;

        public NumberTextParamater() : this(null) { }
        public NumberTextParamater(SharedDataStore? sharedData) : base(sharedData) { }

        public override IEnumerable<string> CreateMaskExoFilter(int keyFrameIndex, ExoOutputDescription desc, ShapeMaskExoOutputDescription shapeMaskDesc)
        {
            int fps = desc.VideoInfo.FPS;
            return new[]
            {
                $"_name=マスク\r\n" +
                $"_disable={(shapeMaskDesc.IsEnabled ? 0 : 1)}\r\n" +
                $"X={shapeMaskDesc.X.ToExoString(keyFrameIndex, "F1",fps)}\r\n" +
                $"Y={shapeMaskDesc.Y.ToExoString(keyFrameIndex, "F1",fps)}\r\n" +
                $"回転={shapeMaskDesc.Rotation.ToExoString(keyFrameIndex, "F2",fps)}\r\n" +
                $"サイズ=100\r\n" +
                $"縦横比=0\r\n" +
                $"ぼかし={shapeMaskDesc.Blur.ToExoString(keyFrameIndex, "F0",fps)}\r\n" +
                $"マスクの反転={(shapeMaskDesc.IsInverted?1:0):F0}\r\n" +
                $"元のサイズに合わせる=0\r\n" +
                $"type=0\r\n" +
                $"name=\r\n" +
                $"mode=0\r\n"
            };
        }

        public override IEnumerable<string> CreateShapeItemExoFilter(int keyFrameIndex, ExoOutputDescription desc)
        {
            return new[]
            {""};
        }

        public override IShapeSource CreateShapeSource(IGraphicsDevicesAndContext devices)
        {
            return new NumberTextSource(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] { Number, DecimalPlaces,FontSize };

        protected override void LoadSharedData(SharedDataStore store)
        {
            var sharedData = store.Load<SharedData>();
            if (sharedData is null)
                return;

            sharedData.CopyTo(this);
        }

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new SharedData(this));
        }

        class SharedData
        {
            public Animation Number { get; } = new Animation(100, 0, 1000);
            public Animation DecimalPlaces { get; } = new Animation(100, 0, 1000);
            public Animation FontSize { get; } = new Animation(100, 0, 1000);
            public SharedData(NumberTextParamater param)
            {
                Number.CopyFrom(param.Number);
                DecimalPlaces.CopyFrom(param.DecimalPlaces);
                FontSize.CopyFrom(param.FontSize);
            }
            public void CopyTo(NumberTextParamater param)
            {
                param.Number.CopyFrom(Number);
                param.DecimalPlaces.CopyFrom(DecimalPlaces);
                param.FontSize.CopyFrom(FontSize);
            }
        }
    }
}
