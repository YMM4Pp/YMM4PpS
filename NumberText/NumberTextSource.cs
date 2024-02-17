using Vortice.Direct2D1;
using Vortice;
using Vortice.DXGI;
using Vortice.DirectWrite;
using Vortice.Win32;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using Vortice.Mathematics;
using static Vortice.Direct2D1.D2D1;
using static Vortice.DirectWrite.DWrite;
using System.Windows;
using System.Numerics;

namespace NumberText
{
    internal class NumberTextSource : IShapeSource
    {
        readonly private IGraphicsDevicesAndContext devices;
        readonly private NumberTextParamater numberTextParameter;

        double number;
        double decimalPlaces;
        string font;
        float fontSize;
        bool sepalate;

        ID2D1SolidColorBrush brush;
        ID2D1CommandList? commandList;

        public ID2D1Image Output => commandList ?? throw new Exception($"{nameof(commandList)}がnullです。事前にUpdateを呼び出す必要があります。");

        public NumberTextSource(IGraphicsDevicesAndContext devices, NumberTextParamater numberTextParameter)
        {
            this.devices = devices;
            this.numberTextParameter = numberTextParameter;
        }

        public void Update(TimelineItemSourceDescription timelineItemSourceDescription)
        {
            var fps = timelineItemSourceDescription.FPS;
            var frame = timelineItemSourceDescription.ItemPosition.Frame;
            var length = timelineItemSourceDescription.ItemDuration.Frame;

            var number = numberTextParameter.Number.GetValue(frame, length, fps);
            var decimalPlaces = numberTextParameter.DecimalPlaces.GetValue(frame, length, fps);
            var font = numberTextParameter.Font;
            var fontSize = (float)numberTextParameter.FontSize.GetValue(frame, length, fps);
            var r = numberTextParameter.Color.R;
            var g = numberTextParameter.Color.G;
            var b = numberTextParameter.Color.B;
            var a = numberTextParameter.Color.A;
            var brush = devices.DeviceContext.CreateSolidColorBrush(new Color4(r,g,b,a));
            var sepalate = numberTextParameter.Sepalate;
            
            if (fontSize == 0) fontSize = 1;
            if (commandList != null && this.number == number && this.decimalPlaces == decimalPlaces && this.fontSize == fontSize && this.font == font && this.brush == brush && this.sepalate == sepalate)
                return;

            var dc = devices.DeviceContext;

            using var formatFactory = DWrite.DWriteCreateFactory<IDWriteFactory>();
            var textFormat = formatFactory.CreateTextFormat(font, fontSize);
            textFormat.SetLineSpacing(LineSpacingMethod.Uniform,0,fontSize*0.8f);
            
            textFormat.WordWrapping = WordWrapping.NoWrap;

            var text = "";
            if (sepalate)
            {
                if (decimalPlaces == 0)
                {
                    text = ((int)number).ToString("N0");
                }
                else
                {
                    text = number.ToString("N" + decimalPlaces);
                }
            }
            else
            {
                text = number.ToString("F" + decimalPlaces);
            }

            using var layoutFactory = DWrite.DWriteCreateFactory<IDWriteFactory>();
            var textLayout = layoutFactory.CreateTextLayout(text, textFormat, fontSize * (text.Length + 1), fontSize);

            commandList?.Dispose();
            commandList = dc.CreateCommandList();

            dc.Target = commandList;
            dc.BeginDraw();
            dc.Clear(null);

            dc.DrawTextLayout(new System.Numerics.Vector2(0, 0), textLayout, brush);

            dc.EndDraw();
            dc.Target = null;
            commandList.Close();

            this.number = number;
            this.decimalPlaces = decimalPlaces;
            this.font = font;
            this.fontSize = fontSize;
            this.brush = brush;
            this.sepalate = sepalate;
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    commandList?.Dispose();
                    brush?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
