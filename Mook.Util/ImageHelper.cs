using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Mook.Util
{
    /// <summary>
    /// 缩略图帮助类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
		/// 计算新尺寸
		/// </summary>
		/// <param name="width">原始宽度</param>
		/// <param name="height">原始高度</param>
		/// <param name="maxWidth">最大新宽度</param>
		/// <param name="maxHeight">最大新高度</param>
		private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            if (maxWidth <= 0) maxWidth = width;
            if (maxHeight <= 0) maxHeight = height;
            int newWidth, newHeight;
            if (width > maxWidth || height > maxHeight)
            {
                decimal factor;
                if ((decimal)width / height > (decimal)maxWidth / maxHeight)
                {
                    factor = (decimal)width / maxWidth;
                }
                else
                {
                    factor = (decimal)height / maxHeight;
                }
                newWidth = Convert.ToInt32(width / factor);
                newHeight = Convert.ToInt32(height / factor);
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="filePath">文件名</param>
        public static ImageFormat GetFormat(string filePath)
        {
            string ext = filePath;
            if (filePath.LastIndexOf(".") > 0)
            {
                ext = filePath.Substring(filePath.LastIndexOf(".") + 1);
            }
            switch (ext.ToLower())
            {
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// 超出最大尺寸裁剪图片
        /// </summary>
        /// <param name="byteData">文件字节数组</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static byte[] MakeThumbnail(byte[] byteData, string fileExt, int maxWidth, int maxHeight)
        {
            var memoryStream = new MemoryStream(byteData);
            var originalImage = Image.FromStream(memoryStream);
            try
            {
                if (originalImage.Width > maxWidth || originalImage.Height > maxHeight)
                {
                    var _newSize = ResizeImage(originalImage.Width, originalImage.Height, maxWidth, maxHeight);
                    var displayImage = new Bitmap(originalImage, _newSize);

                    using (var ms = new MemoryStream())
                    {
                        displayImage.Save(ms, GetFormat(fileExt));
                        var buffer = new byte[ms.Length];
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
            }
            finally
            {
                memoryStream.Close();
                originalImage.Dispose();
            }
            return byteData;
        }

        /// <summary>
        /// 根据X轴和Y轴裁剪图片
        /// </summary>
        /// <param name="byteData">文件字节数组</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="maxWidth">缩略图宽度</param>
        /// <param name="maxHeight">缩略图高度</param>
        /// <param name="cropWidth">裁剪宽度</param>
        /// <param name="cropHeight">裁剪高度</param>
        /// <param name="X">X轴</param>
        /// <param name="Y">Y轴</param>
        public static byte[] MakeThumbnail(byte[] byteData, string fileExt, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
        {
            var memoryStream = new MemoryStream(byteData);
            var originalImage = Image.FromStream(memoryStream);
            var b = new Bitmap(cropWidth, cropHeight);
            try
            {
                using (var g = Graphics.FromImage(b))
                {
                    //设置高质量插值法
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //设置高质量,低速度呈现平滑程度
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //清空画布并以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, cropWidth, cropHeight), X, Y, cropWidth, cropHeight, GraphicsUnit.Pixel);
                    var displayImage = new Bitmap(b, maxWidth, maxHeight);
                    using (var ms = new MemoryStream())
                    {
                        displayImage.Save(ms, GetFormat(fileExt));
                        var buffer = new byte[ms.Length];
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                originalImage.Dispose();
                b.Dispose();
            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="byteData">文件字节数组</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>
        public static byte[] MakeThumbnail(byte[] byteData, string fileExt, int width, int height, string mode)
        {
            var memoryStream = new MemoryStream(byteData);
            var originalImage = Image.FromStream(memoryStream);
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（补白）
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    else
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            var b = new Bitmap(towidth, toheight);
            try
            {
                //新建一个画板
                var g = Graphics.FromImage(b);
                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //清空画布并以透明背景色填充
                g.Clear(Color.White);
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

                using (var ms = new MemoryStream())
                {
                    b.Save(ms, GetFormat(fileExt));
                    var buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                memoryStream.Close();
                originalImage.Dispose();
                b.Dispose();
            }
        }

        /// <summary>
        /// 图片水印
        /// </summary>
        /// <param name="byteData">图片字节数组</param>
        /// <param name="fileExt">图片扩展名</param>
        /// <param name="filePath">水印文件物理路径</param>
        /// <param name="location">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="transparency">水印的透明度 1--10 10为不透明</param>
        public static byte[] ImageWatermark(byte[] byteData, string fileExt, string filePath, int location, int quality, int transparency)
        {
            var memoryStream = new MemoryStream(byteData);
            var img = Image.FromStream(memoryStream);
            if (!File.Exists(filePath))
            {
                return byteData;
            }
            var g = Graphics.FromImage(img);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var watermark = new Bitmap(filePath);
            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
            {
                return byteData;
            }

            var imageAttributes = new ImageAttributes();
            var colorMap = new ColorMap()
            {
                OldColor = Color.FromArgb(255, 0, 255, 0),
                NewColor = Color.FromArgb(0, 0, 0, 0)
            };
            ColorMap[] remapTable = { colorMap };
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float toTransparency = 0.5F;
            if (transparency >= 1 && transparency <= 10)
            {
                toTransparency = (transparency / 10.0F);
            }

            float[][] colorMatrixElements = {
                new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                new float[] {0.0f,  0.0f,  0.0f, toTransparency, 0.0f},
                new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
            };
            var colorMatrix = new ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;
            switch (location)
            {
                case 1:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (var codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    ici = codec;
                }
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
            {
                quality = 80;
            }

            qualityParam[0] = quality;

            var encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;
            try
            {
                using (var ms = new MemoryStream())
                {
                    if (ici != null)
                    {
                        img.Save(ms, ici, encoderParams);
                    }
                    else
                    {
                        img.Save(ms, GetFormat(fileExt));
                    }
                    var buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
            finally
            {
                g.Dispose();
                img.Dispose();
                watermark.Dispose();
                imageAttributes.Dispose();
            }
        }

        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="byteData">图片字节数组</param>
        /// <param name="fileExt">图片扩展名</param>
        /// <param name="text">水印文字</param>
        /// <param name="location">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="fontName">字体</param>
        /// <param name="fontSize">字体大小</param>
        public static byte[] LetterWatermark(byte[] byteData, string fileExt, string text, int location, int quality, string fontName, int fontSize)
        {
            var memoryStream = new MemoryStream(byteData);
            var img = Image.FromStream(memoryStream);
            var g = Graphics.FromImage(img);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            var drawFont = new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(text, drawFont);

            float xpos = 0;
            float ypos = 0;
            switch (location)
            {
                case 1:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 2:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case 3:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 4:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            g.DrawString(text, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
            g.DrawString(text, drawFont, new SolidBrush(Color.Black), xpos, ypos);

            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    ici = codec;
                }
            }
            var encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
            {
                quality = 80;
            }

            qualityParam[0] = quality;

            var encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;
            try
            {
                using (var ms = new MemoryStream())
                {
                    if (ici != null)
                    {
                        img.Save(ms, ici, encoderParams);
                    }
                    else
                    {
                        img.Save(ms, GetFormat(fileExt));
                    }
                    var buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
            finally
            {
                g.Dispose();
                img.Dispose();
            }
        }
    }
}
