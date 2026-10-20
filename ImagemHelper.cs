using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Forms_Fruteira
{
    public static class ImagemHelper
    {
        // Reduz mantendo a proporção (nunca aumenta)
        public static Image Redimensionar(Image original, int tamanhoMax)
        {
            double escala = Math.Min(1.0, Math.Min(
                (double)tamanhoMax / original.Width,
                (double)tamanhoMax / original.Height));

            int w = Math.Max(1, (int)(original.Width * escala));
            int h = Math.Max(1, (int)(original.Height * escala));

            var nova = new Bitmap(w, h);
            using (var g = Graphics.FromImage(nova))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.White);              // PNG transparente vira fundo branco
                g.DrawImage(original, 0, 0, w, h);
            }
            return nova;
        }

        // Miniatura quadrada para a lista do Estoque, sem distorcer
        public static Image Miniatura(Image original, int lado)
        {
            var mini = new Bitmap(lado, lado);
            using (var reduzida = Redimensionar(original, lado))
            using (var g = Graphics.FromImage(mini))
            {
                g.Clear(Color.White);
                g.DrawImage(reduzida, (lado - reduzida.Width) / 2, (lado - reduzida.Height) / 2);
            }
            return mini;
        }

        public static byte[] ParaBytes(Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static Image DeBytes(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var temp = Image.FromStream(ms))
                return new Bitmap(temp);           // cópia que não depende do stream
        }
    }
}