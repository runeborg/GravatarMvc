using System.Web.Mvc;
using System.Text;
using System.Web;
using System.Collections.Generic;

namespace System.Web.Mvc
{
	/// <summary>
	/// Specifies what displays if the email has no matching Gravatar image.
	/// </summary>
	public enum DefaultGravatar
	{
		/// <summary>
		/// Use the default image (Gravatar logo)
		/// </summary>
		GravatarLogo,
		/// <summary>
		/// Do not load any image if none is associated with the email, instead return an HTTP 404 (File Not Found) response.
		/// </summary>
		None,
		/// <summary>
		/// A simple, cartoon-style silhouetted outline of a person (does not vary by email).
		/// </summary>
		MysteryMan,
		/// <summary>
		/// A geometric pattern based on an email.
		/// </summary>
		IdentIcon,
		/// <summary>
		/// A generated 'monster' with different colors, faces, etc.
		/// </summary>
		MonsterId,
		/// <summary>
		/// Generated faces with differing features and backgrounds.
		/// </summary>
		Wavatar,
		/// <summary>
		/// Generated, 8-bit arcade-style pixelated faces.
		/// </summary>
		Retro
	}

	/// <summary>
	/// If the requested email hash does not have an image meeting the requested rating level, then the default image is returned (or the specified default).
	/// </summary>
	public enum GravatarRating
	{
		/// <summary>
		///  Default rating (G)
		/// </summary>
		Default,
		/// <summary>
		/// Suitable for display on all websites with any audience type.
		/// </summary>
		G,
		/// <summary>
		/// May contain rude gestures, provocatively dressed individuals, the lesser swear words, or mild violence.
		/// </summary>
		PG,
		/// <summary>
		/// May contain such things as harsh profanity, intense violence, nudity, or hard drug use.
		/// </summary>
		R,
		/// <summary>
		/// May contain hardcore sexual imagery or extremely disturbing violence.
		/// </summary>
		X
	}

	public class GravatarGenerator
	{
		private string _Email { get; set; }
		private int _Size { get; set; }
		private string _DefaultImage { get; set; }
		private bool _AppendFileType { get; set; }
		private bool _ForceDefaultImage { get; set; }
		private GravatarRating _DisplayRating { get; set; }
		private DefaultGravatar _DefaultDisplay { get; set; }
		private bool _UseHttps { get; set; }

		public GravatarGenerator(string email, bool useHttps)
		{
			_Email = email;
			_UseHttps = useHttps;
		}

		public string Url
		{
			get
			{
				string prefix = this._UseHttps ? "https://" : "http://";
				string url = prefix + "gravatar.com/avatar/" + Encode(Encoding.UTF8);

				if (this._AppendFileType)
					url += ".jpg";

				url += BuildUrlParams();

				return url;
			}
		}

		public GravatarGenerator Size(int size)
		{
			if (size < 0 || size > 512)
				throw new ArgumentOutOfRangeException("size", "Image size must be between 1 and 512");

			this._Size = size;
			return this;
		}

		public GravatarGenerator DefaultImage(string defaultImage)
		{
			this._DefaultImage = defaultImage;
			return this;
		}

		public GravatarGenerator DefaultImage(DefaultGravatar defaultImage)
		{
			this._DefaultDisplay = defaultImage;
			return this;
		}

		public GravatarGenerator Rating(GravatarRating rating)
		{
			this._DisplayRating = rating;
			return this;
		}

		public GravatarGenerator AppendFileType()
		{
			this._AppendFileType = true;
			return this;
		}

		public GravatarGenerator ForceDefaultImage()
		{
			this._ForceDefaultImage = true;
			return this;
		}

		private string Encode(Encoding encoding)
		{
			System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] hash = encoding.GetBytes(_Email);
			hash = x.ComputeHash(hash);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("x2"));
			}

			return sb.ToString();
		}

		private string GetRatingParam()
		{
			switch (this._DisplayRating)
			{
				case GravatarRating.G: return "r=g";
				case GravatarRating.PG: return "r=pg";
				case GravatarRating.R: return "r=r";
				case GravatarRating.X: return "r=x";
				default: return null;
			}
		}

		private string GetDefaultImageParam()
		{
			switch (this._DefaultDisplay)
			{
				case DefaultGravatar.IdentIcon: return "d=identicon";
				case DefaultGravatar.MonsterId: return "d=monsterid";
				case DefaultGravatar.MysteryMan: return "d=mm";
				case DefaultGravatar.None: return "d=404";
				case DefaultGravatar.Retro: return "d=retro";
				case DefaultGravatar.Wavatar: return "d=wavatar";
				default: return string.IsNullOrWhiteSpace(this._DefaultImage) ? null : "d=" + HttpUtility.UrlEncode(this._DefaultImage); ;
			}
		}

		private string BuildUrlParams()
		{
			if (this._Size < 0 || this._Size > 512)
				throw new ArgumentOutOfRangeException("Size", "Image size must be between 1 and 512");

			string defaultImageParam = GetDefaultImageParam();
			string ratingParam = GetRatingParam();

			List<string> urlParams = new List<string>();
			if (this._Size > 0)
				urlParams.Add("s=" + this._Size);
			if (!string.IsNullOrWhiteSpace(ratingParam))
				urlParams.Add(ratingParam);
			if (!string.IsNullOrWhiteSpace(defaultImageParam))
				urlParams.Add(defaultImageParam);
			if (this._ForceDefaultImage)
				urlParams.Add("f=y");

			if (urlParams.Count == 0)
				return "";

			string paramString = "?";

			for (int i = 0; i < urlParams.Count; ++i)
			{
				paramString += urlParams[i];
				if (i < urlParams.Count - 1)
					paramString += "&";
			}

			return paramString;
		}
	}

	public static class GravatarExtension
	{
		public static GravatarGenerator GravatarGenerator(this UrlHelper helper, string email)
		{
			return new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection);
		}

		public static GravatarGenerator GravatarGenerator(this UrlHelper helper, string email, int size)
		{
			return new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection).Size(size);
		}

		public static string Gravatar(this UrlHelper helper, string email, int size)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection).Size(size);
			return gravatar.Url;
		}

		public static string Gravatar(this UrlHelper helper, string email, int size, DefaultGravatar defaultImage)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection)
				.Size(size)
				.DefaultImage(defaultImage);
			return gravatar.Url;
		}

		public static string Gravatar(this UrlHelper helper, string email, int size, string defaultImage)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection)
				.Size(size)
				.DefaultImage(defaultImage);
			return gravatar.Url;
		}

		public static string Gravatar(this UrlHelper helper, string email, int size, DefaultGravatar defaultImage, GravatarRating rating)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection)
				.Size(size)
				.Rating(rating)
				.DefaultImage(defaultImage);
			return gravatar.Url;
		}

		public static string Gravatar(this UrlHelper helper, string email, int size, string defaultImage, GravatarRating rating)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection)
				.Size(size)
				.Rating(rating)
				.DefaultImage(defaultImage);
			return gravatar.Url;
		}
	}
}