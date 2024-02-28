
class ImageHelper
{

    public static string SaveArticleImage(string? ImgStr)
    {

        return Save(ImgStr, Common.ArticlesFolder);
    }
    public static string Save(string ImgStr, string ImgFolder)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(ImgStr))
                return "";
            if (ImgStr.Length < 100)
                return ImgStr;
            var image_NewGuid = Guid.NewGuid().ToString();
            var path = Path.Combine(Directory.GetCurrentDirectory(), ImgFolder);

            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            string imageName = image_NewGuid + ".webp";

            //set the image path
            string imgPath = Path.Combine(path, imageName);

            string[] str = ImgStr.Split(',');
            byte[] bytes = Convert.FromBase64String(str[1]);

            File.WriteAllBytes(imgPath, bytes);
            return imageName;
        }
        catch (Exception ex)
        {
        }
        finally
        {
        }
        return "";
    }

}