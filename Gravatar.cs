using System;
using System.Web;
using System.Text;
using System.Web.Mvc;
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

	/// <summary>
	/// Generates a Gravatar url
	/// </summary>
	public class GravatarGenerator
	{
		/// <summary>
		/// Email to generate a Gravatar image for
		/// </summary>
		private string _Email { get; set; }
		/// <summary>
		/// The size of the image in pixels. Defaults to 80px
		/// </summary>
		private int _Size { get; set; }
		/// <summary>
		/// A default image to fall back to.
		/// </summary>
		private string _DefaultImage { get; set; }
		/// <summary>
		/// Wether to append a file ending to the url (.jpg).
		/// </summary>
		private bool _AppendFileType { get; set; }
		/// <summary>
		/// Force the default image to display.
		/// </summary>
		private bool _ForceDefaultImage { get; set; }
		/// <summary>
		/// Image rating to display for. See <see cref="Gravatar.GravatarRating"/> for details.
		/// </summary>
		private GravatarRating _DisplayRating { get; set; }
		/// <summary>
		/// How to generate a default image if no gravatar exists for the email. See <see cref="Gravatar.DefaultGravatar"/> for details.
		/// </summary>
		private DefaultGravatar _DefaultDisplay { get; set; }
		/// <summary>
		/// If https should be used.
		/// </summary>
		private bool _UseHttps { get; set; }

		/// <summary>
		/// Creates a GravatarGenerator.
		/// </summary>
		/// <param name="email">Email to generate Gravatar for.</param>
		/// <param name="useHttps">Wether to use https or not.</param>
		public GravatarGenerator(string email, bool useHttps)
		{
			_Email = email;
			_UseHttps = useHttps;
		}

		/// <summary>
		/// Gets the Url for the Gravatar
		/// </summary>
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

		/// <summary>
		/// Sets the size of the Gravatar.
		/// </summary>
		/// <param name="size">Size in pixels between 1 and 512.</param>
		public GravatarGenerator Size(int size)
		{
			if (size < 0 || size > 512)
				throw new ArgumentOutOfRangeException("size", "Image size must be between 1 and 512");

			this._Size = size;
			return this;
		}

		/// <summary>
		/// A default image to fall back to.
		/// </summary>
		/// <param name="defaultImage">A url to use as a default image.</param>
		public GravatarGenerator DefaultImage(string defaultImage)
		{
			this._DefaultImage = defaultImage;
			return this;
		}

		/// <summary>
		/// How to generate a default image if no gravatar exists for the email. See <see cref="Gravatar.DefaultGravatar"/> for details.
		/// </summary>
		/// <param name="defaultImage">What type of default image to generate.</param>
		public GravatarGenerator DefaultImage(DefaultGravatar defaultImage)
		{
			this._DefaultDisplay = defaultImage;
			return this;
		}

		/// <summary>
		/// Image rating to display for. See <see cref="Gravatar.GravatarRating"/> for details.
		/// </summary>
		/// <param name="defaultImage">The rating to filter Gravatars for.</param>
		public GravatarGenerator Rating(GravatarRating rating)
		{
			this._DisplayRating = rating;
			return this;
		}

		/// <summary>
		/// Wether to append a file ending to the url (.jpg).
		/// </summary>
		public GravatarGenerator AppendFileType()
		{
			this._AppendFileType = true;
			return this;
		}

		/// <summary>
		/// Force the default image to display.
		/// </summary>
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
			if(!string.IsNullOrWhiteSpace(this._DefaultImage))
				return "d=" + HttpUtility.UrlEncode(this._DefaultImage);

			switch (this._DefaultDisplay)
			{
				case DefaultGravatar.IdentIcon: return "d=identicon";
				case DefaultGravatar.MonsterId: return "d=monsterid";
				case DefaultGravatar.MysteryMan: return "d=mm";
				case DefaultGravatar.None: return "d=404";
				case DefaultGravatar.Retro: return "d=retro";
				case DefaultGravatar.Wavatar: return "d=wavatar";
				default: return null;
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
		/// <summary>
		/// Gets a <see cref="Gravatar.GravatarGenerator"/> object.
		/// </summary>
		/// <param name="helper">UrlHelper object.</param>
		/// <param name="email">Email to generate Gravatar for.</param>
		/// <returns>A GravatarGenerator object.</returns>
		public static GravatarGenerator GravatarGenerator(this UrlHelper helper, string email)
		{
			return new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection);
		}

		/// <summary>
		/// Gets a <see cref="Gravatar.GravatarGenerator"/> object.
		/// </summary>
		/// <param name="helper">UrlHelper objec.t</param>
		/// <param name="email">Email to generate Gravatar for.</param>
		/// <param name="size">The size in pixels, between 1 and 512.</param>
		/// <returns>A GravatarGenerator object</returns>
		public static GravatarGenerator GravatarGenerator(this UrlHelper helper, string email, int size)
		{
			return new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection).Size(size);
		}

		/// <summary>
		/// Gets a Gravatar Url as string.
		/// </summary>
		/// <param name="helper">UrlHelper object.</param>
		/// <param name="email">Email to generate Gravatar for.</param>
		/// <param name="size">The size in pixels, between 1 and 512.</param>
		/// <returns>A Gravatar Url</returns>
		public static string Gravatar(this UrlHelper helper, string email, int size)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection).Size(size);
			return gravatar.Url;
		}

		/// <summary>
		/// Gets a Gravatar Url as string.
		/// </summary>
		/// <param name="helper">UrlHelper object.</param>
		/// <param name="email">Email to generate Gravatar for.</param>
		/// <param name="size">The size in pixels, between 1 and 512.</param>
		/// <param name="defaultImage">A default Gravatar generation policy. See <see cref="Gravatar.DefaultGravatar"/> for details.</param>
		/// <returns>A Gravatar Url</returns>
		public static string Gravatar(this UrlHelper helper, string email, int size, DefaultGravatar defaultImage)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection)
				.Size(size)
				.DefaultImage(defaultImage);
			return gravatar.Url;
		}

		/// <summary>
		/// Gets a Gravatar Url as string.
		/// </summary>
		/// <param name="helper">UrlHelper object.</param>
		/// <param name="email">Email to generate Gravatar for.</param>
		/// <param name="size">The size in pixels, between 1 and 512.</param>
		/// <param name="defaultImage">An Url to a default image to use if no Gravatar exists.</param>
		/// <returns>A Gravatar Url</returns>
		public static string Gravatar(this UrlHelper helper, string email, int size, string defaultImage)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection)
				.Size(size)
				.DefaultImage(defaultImage);
			return gravatar.Url;
		}

		/// <summary>
		/// Gets a Gravatar Url as string.
		/// </summary>
		/// <param name="helper">UrlHelper object.</param>
		/// <param name="email">Email to generate Gravatar for.</param>
		/// <param name="size">The size in pixels, between 1 and 512.</param>
		/// <param name="defaultImage">A default Gravatar generation policy. See <see cref="Gravatar.DefaultGravatar"/> for details.</param>
		/// <param name="rating">Image rating to display for. See <see cref="Gravatar.GravatarRating"/> for details.</param>
		/// <returns>A Gravatar Url</returns>
		public static string Gravatar(this UrlHelper helper, string email, int size, DefaultGravatar defaultImage, GravatarRating rating)
		{
			GravatarGenerator gravatar = new GravatarGenerator(email, helper.RequestContext.HttpContext.Request.IsSecureConnection)
				.Size(size)
				.Rating(rating)
				.DefaultImage(defaultImage);
			return gravatar.Url;
		}

		/// <summary>
		/// Gets a Gravatar Url as string.
		/// </summary>
		/// <param name="helper">UrlHelper object.</param>
		/// <param name="email">Email to generate Gravatar for.</param>
		/// <param name="size">The size in pixels, between 1 and 512.</param>
		/// <param name="defaultImage">An Url to a default image to use if no Gravatar exists.</param>
		/// <param name="rating">Image rating to display for. See <see cref="Gravatar.GravatarRating"/> for details.</param>
		/// <returns>A Gravatar Url</returns>
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