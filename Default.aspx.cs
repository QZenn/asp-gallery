using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public partial class _Default : System.Web.UI.Page
{
    private static System.Collections.ArrayList lockersList = new System.Collections.ArrayList();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        string pathToThumbnailsFolder = Server.MapPath(@"generated_thumbs\");
        string pathToSourceFolder = Server.MapPath("images_for_gallery");

        int thumbMaxWidth = 180;
        int thumbMaxHeight = 180;
        
               
        ArrayList generatedValues = GenerateThumbnailsForFolder(pathToSourceFolder, pathToThumbnailsFolder, thumbMaxWidth, thumbMaxHeight);
        System.Collections.ArrayList sourceValues = new System.Collections.ArrayList();
        foreach (ThumbnailData data in generatedValues)
        {
            sourceValues.Add(new ThumbnailData(
                ("images_for_gallery/" + data.pathToSourceImage.Substring(data.pathToSourceImage.LastIndexOf("\\") + 1)),
                ("generated_thumbs/" + data.pathToThumbnailImage.Substring(data.pathToThumbnailImage.LastIndexOf("\\") + 1)), 
                data.filename));
        }

        RepeaterThumbnails.DataSource = sourceValues;
        RepeaterThumbnails.DataBind();
    }

    public class ThumbnailData
    {
        public string pathToSourceImage {get; set;}
        public string pathToThumbnailImage { get; set; }
        public string filename { get; set; }

        public ThumbnailData(string pathToSourceImage, string pathToThumbnailImage, string filename)
        {
            this.pathToSourceImage = pathToSourceImage;
            this.pathToThumbnailImage = pathToThumbnailImage;
            this.filename = filename;
        }
    }

    private string rigthTrim (string str)
    {
        int length = str.Length;
        int end = str.LastIndexOf(" ");
        for (int i = length; i >= end; --i)
        {
            if (str.EndsWith(" "))
            {
                str.Remove(i);
            }
        }
        return str;
    }

    private string leftTrim(string str)
    {
        int length = str.Length;
        int end = str.IndexOf(" ");
        for (int i = 1; i <= end; ++i)
        {
            if (str.StartsWith(" "))
            {
                str.Remove(i);
            }
        }
        return str;
    }


    public string Trim(string str)
    {
        return leftSpaceTrim(rightSpaceTrim(str));
    }
    
    private static string rightSpaceTrim(string str)
        {
            int startPosition = str.Length - 1;

            while (str[startPosition].Equals(' '))
            {
                startPosition--;
            }

            str = str.Substring(0, startPosition + 1);
            return str;
        }


        private static string leftSpaceTrim(string str)
        {
            int startPosition = 0;

            while (str[startPosition].Equals(' ') && (startPosition < str.Length))
            {
                startPosition++;
            }

            str = str.Substring(startPosition);
            return str;
        }

        static void Main(string[] args)
        {

            String str = "Хер";
            str = rightSpaceTrim(str);
            str = leftSpaceTrim(str);
            str = rightSpaceTrim(str);
        }


    private ArrayList GenerateThumbnailsForFolder(string pathToFolder, string ThumbnailsFolderPath, int thumbMaxWidth, int thumbMaxHeight)
    {
        ArrayList generatedThumbnails = new ArrayList();
        string[] files = Directory.GetFiles(pathToFolder, "*", SearchOption.TopDirectoryOnly);
        foreach (string sourcePath in files)
        {
            if (isImage(Path.GetExtension(sourcePath)))
            {
                string filename = (sourcePath.Substring(sourcePath.LastIndexOf("\\") + 1));
                filename = filename.Substring(0, (filename.Length - Path.GetExtension(sourcePath).Length));
                string destinationPath = (ThumbnailsFolderPath + filename + ".jpg");

                lock (Locker.lockString(destinationPath))
                {
                    if (needGenerateThumbnail(sourcePath, destinationPath))
                    {
                        GenerateThumbnailForImage(sourcePath, destinationPath, thumbMaxWidth, thumbMaxHeight);
                    }
                    Locker.unlockString(destinationPath);
                }

                generatedThumbnails.Add(new ThumbnailData(sourcePath, destinationPath, filename));
            }
        }

        return generatedThumbnails;
    }

    private static bool isImage(string ext)
    {
        ext = ext.ToLower();
        return (ext == ".jpg") || 
               (ext == ".jpeg") || 
               (ext == ".gif") || 
               (ext == ".png") || 
               (ext == ".bmp");
    }

    private Boolean needGenerateThumbnail(string sourcePath, string destinationPath)
    {
        Boolean needNewThumbnail = true;
        if (File.Exists(sourcePath)&&File.Exists(destinationPath))
        {
            int compareResult = DateTime.Compare(
                File.GetLastWriteTime(sourcePath),
                File.GetLastWriteTime(destinationPath));
            if (compareResult > 0)
            {
                needNewThumbnail = true;
            }
            else
            {
                needNewThumbnail = false;
            }
        }
        return needNewThumbnail;
    }

    private static void GenerateThumbnailForImage(string sourcePath, string destinationPath, int thumbMaxWidth, int thumbMaxHeight)
    {
        System.Drawing.Image image = System.Drawing.Image.FromFile(sourcePath);
        int[] imageDimensionsXY = new int[2];
        imageDimensionsXY[0] = image.Width;
        imageDimensionsXY[1] = image.Height;
        int[] thumbnailSizeXY = CalculateThumbnailSize(imageDimensionsXY, thumbMaxWidth, thumbMaxHeight);
        System.Drawing.Bitmap bitmap = ResizeImage(image, thumbnailSizeXY[0], thumbnailSizeXY[1]);
        SaveImageToJpegFile(bitmap, destinationPath);
        bitmap.Dispose();
        image.Dispose();
    }

    private static int[] CalculateThumbnailSize(int[] imageDimensionsXY, int thumbMaxWidth, int thumbMaxHeight)
    {
        int srcWidth = imageDimensionsXY[0];
        int srcHeight = imageDimensionsXY[1];
        int thumbHeight;
        int thumbWidth;
        if (srcHeight > srcWidth)
        {
            thumbHeight = (int)thumbMaxHeight;
            thumbWidth = (int)((double)thumbMaxHeight * ((double)srcWidth / (double)srcHeight));

        }
        else
        {
            thumbWidth = (int)thumbMaxWidth;
            thumbHeight = (int)((double)thumbMaxWidth * ((double)srcHeight / (double)srcWidth));
        }
        int[] thumbnailSizeXY = new int[2];
        thumbnailSizeXY[0] = thumbWidth;
        thumbnailSizeXY[1] = thumbHeight;

        return thumbnailSizeXY;
    }

    private static void SaveImageToJpegFile(System.Drawing.Bitmap bitmap, string path)
    {
        System.Drawing.Imaging.ImageCodecInfo imagecodec;
        System.Drawing.Imaging.EncoderParameters encoderparams;
        imagecodec = GetEncoderInfo("image/jpeg");
        encoderparams = new System.Drawing.Imaging.EncoderParameters(1);
        encoderparams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
         //Multithreading debug code
#if DEBUG
        if (File.Exists(path))
        {
            File.WriteAllText((path + ".error"), "error");
        }
#endif
        bitmap.Save(path, imagecodec, encoderparams);
    }

    /// <summary>
    /// Returns Bitmap with high quality image resize.
    /// </summary>
    /// <param name="image">Image object</param>
    /// <param name="requiredWidth">Horizontal size</param>
    /// <param name="requiredHeight">Vertical size</param>
    /// <returns></returns>
    private static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int requiredWidth, int requiredHeight)
    {
        int srcWidth = image.Width;
        int srcHeight = image.Height;
        System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(requiredWidth, requiredHeight);
        System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, requiredWidth, requiredHeight);
        graphics.DrawImage(image, rectangle, 0, 0, srcWidth, srcHeight, System.Drawing.GraphicsUnit.Pixel);
        return bitmap;
    }

    private static ImageCodecInfo GetEncoderInfo(String mimeType)
    {
        int j;
        ImageCodecInfo[] encoders;
        encoders = ImageCodecInfo.GetImageEncoders();
        for (j = 0; j < encoders.Length; ++j)
        {
            if (encoders[j].MimeType == mimeType)
                return encoders[j];
        }
        return null;
    }

    private class Locker
    {
        private static Object locker = new Object();
        public string lockStr;

        private Locker(string str){
            lockStr = str;
        }
        public static Locker lockString(string str)
        {
            lock (locker)
            {
                Locker returnvalue = new Locker("");
                Boolean pathAlreadyLocked = false;
                ArrayList stringToDelete = new ArrayList();
                foreach (Locker path in lockersList)
                {
                    if (String.Equals(path.lockStr, str))
                    {
                        if (pathAlreadyLocked)
                        {
                            stringToDelete.Add(path);
                        }
                        else
                        {
                            pathAlreadyLocked = true;
                            returnvalue = path;
                        }
                    }
                }
                foreach (Locker item in stringToDelete)
                {
                    lockersList.Remove(item);
                }
                if (!pathAlreadyLocked)
                {
                    returnvalue = new Locker(str);
                    lockersList.Add(returnvalue);
                }

                return returnvalue;
            }
        }

        public static void unlockString(string str)
        {
            lock (locker)
            {
                ArrayList stringToDelete = new ArrayList();
                foreach (Locker path in lockersList)
                {
                    if (String.Equals(path.lockStr, str))
                    {
                        stringToDelete.Add(path);
                    }
                }
                foreach (Locker item in stringToDelete)
                {
                    lockersList.Remove(item);
                }
            }
        }
    }
}
