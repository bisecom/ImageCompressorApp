using System;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Generic;

namespace DZ_2_ImgProcessing
{
    static class Operating
    {
        static Operating() {}

        public static void SaveImg(ParallObj obj)
        {
            if (obj.ImgQualityObj < 0 || obj.ImgQualityObj > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, obj.ImgQualityObj);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            Image imgTemp = Image.FromFile(obj.ImgPathObj);
            imgTemp.Save(obj.pathDestinationObj + @"\" + Path.GetFileName(obj.ImgPathObj), jpegCodec, encoderParams);

        }
        
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

      
        public static void SaveImgAndGray(ParallObj obj)
        {
            if (obj.ImgQualityObj < 0 || obj.ImgQualityObj > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, obj.ImgQualityObj);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            Image imgTemp = Image.FromFile(obj.ImgPathObj);
            string tempPath = obj.pathDestinationObj + @"\" + Path.GetFileName(obj.ImgPathObj);
            imgTemp.Save(tempPath, jpegCodec, encoderParams);
            

            Bitmap image = new Bitmap(obj.pathDestinationObj + @"\" + Path.GetFileName(obj.ImgPathObj));
            Bitmap grayScale = new Bitmap(image.Width, image.Height);

            for (Int32 y = 0; y < grayScale.Height; y++)
                for (Int32 x = 0; x < grayScale.Width; x++)
                {
                    System.Drawing.Color c = image.GetPixel(x, y);

                    Int32 gs = (Int32)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);

                    grayScale.SetPixel(x, y, System.Drawing.Color.FromArgb(gs, gs, gs));
                }
            string tempPath1 = obj.pathDestinationObj + @"\_Gray_" + Path.GetFileName(obj.ImgPathObj);
            grayScale.Save(tempPath1, ImageFormat.Jpeg);


        }

    }
    
}
