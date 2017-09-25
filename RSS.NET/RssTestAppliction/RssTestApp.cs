using System;
using System.IO;

namespace Rss
{
	/// <summary>
	/// Summary description for RssTestApp.
	/// </summary>
	class RssTestApp
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			RssFeed r = new RssFeed();

			r.Version = RssVersion.RSS20;

			RssItem ri1a = new RssItem();
			ri1a.Author = "Test Author 1a";
			ri1a.Title = "Test Title 1a";
			ri1a.Description = "Test Description 1a";
			ri1a.Link = new Uri("http://www.yourserver.com/");
			ri1a.PubDate = DateTime.Now;

			RssItem ri1b = new RssItem();
			ri1b.Author = "Test Author 1b";
			ri1b.Title = "Test Title 1b";
			ri1b.Description = "Test Description 1b";
			ri1b.Link = new Uri("http://www.yourserver.com/");
			ri1b.PubDate = DateTime.Now;

			RssChannel rc1 = new RssChannel();
			rc1.Items.Add(ri1a);
			rc1.Items.Add(ri1b);
			rc1.Title = "Test Channel Title 1";
			rc1.Description = "Test Channel Description 1";
			rc1.Link = new Uri("http://www.yourserver.com/channel.html");
			rc1.PubDate = DateTime.Now;

			r.Channels.Add(rc1);

			RssPhotoAlbumCategoryPhotoPeople pacpp = new RssPhotoAlbumCategoryPhotoPeople("John Doe");
			
			RssPhotoAlbumCategoryPhoto pacp1 = new RssPhotoAlbumCategoryPhoto(DateTime.Now.Subtract(new TimeSpan(2, 12, 0, 0)), "Test Photo Description 1", new Uri("http://www.yourserver.com/PhotoAlbumWeb/GetPhoto.aspx?PhotoID=123"), pacpp);
			RssPhotoAlbumCategoryPhoto pacp2 = new RssPhotoAlbumCategoryPhoto(DateTime.Now.Subtract(new TimeSpan(2, 10, 0, 0)), "Test Photo Description 2", new Uri("http://www.yourserver.com/PhotoAlbumWeb/GetPhoto.aspx?PhotoID=124"));
			RssPhotoAlbumCategoryPhoto pacp3 = new RssPhotoAlbumCategoryPhoto(DateTime.Now.Subtract(new TimeSpan(2, 10, 0, 0)), "Test Photo Description 2", new Uri("http://www.yourserver.com/PhotoAlbumWeb/GetPhoto.aspx?PhotoID=125"));
			RssPhotoAlbumCategoryPhotos pacps = new RssPhotoAlbumCategoryPhotos();
			pacps.Add(pacp1);
			pacps.Add(pacp2);
			
			RssPhotoAlbumCategory pac1 = new RssPhotoAlbumCategory("Test Photo Album Category 1", "Test Photo Album Category Description 1", DateTime.Now.Subtract(new TimeSpan(5, 10, 0, 0)), DateTime.Now, pacps);
			RssPhotoAlbumCategory pac2 = new RssPhotoAlbumCategory("Test Photo Album Category 2", "Test Photo Album Category Description 2", DateTime.Now.Subtract(new TimeSpan(9, 10, 0, 0)), DateTime.Now, pacp3);
			RssPhotoAlbumCategories pacs = new RssPhotoAlbumCategories();
			pac1.BindTo(ri1a.GetHashCode());
			pac2.BindTo(ri1b.GetHashCode());
			pacs.Add(pac1);
			pacs.Add(pac2);

			RssPhotoAlbum pa = new RssPhotoAlbum(new Uri("http://your.web.server/PhotoAlbumWeb"), pacs);

			pa.BindTo(rc1.GetHashCode());

			r.Modules.Add(pa);

			RssItem ri2 = new RssItem();
			ri2.Author = "Test Author 2";
			ri2.Title = "Test Title 2";
			ri2.Description = "Test Description 2";
			ri2.Link = new Uri("http://www.yourotherserver.com/");
			ri2.PubDate = DateTime.Now;

			RssChannel rc2 = new RssChannel();
			rc2.Items.Add(ri2);
			rc2.Title = "Test Channel Title 2";
			rc2.Description = "Test Channel Description 2";
			rc2.Link = new Uri("http://www.yourotherserver.com/channel.html");
			rc2.PubDate = DateTime.Now;

			r.Channels.Add(rc2);
			
			r.Write("out.xml");

			RssBlogChannel rbc = new RssBlogChannel(new Uri("http://www.google.com"), new Uri("http://www.google.com"), new Uri("http://www.google.com"), new Uri("http://www.google.com"));
		}
	}
}
