#GravatarMvc#

> Gravatar wrapper for ASP.NET MVC. Feel free to use it any way you want.

###Usage###
	<img src="@Url.Gravatar("my@email.com")" alt="" /> // Displays a Gravatar for my@email.com
	<img src="@Url.Gravatar("my@email.com", 100)" alt="" /> // Sets the size to 100x100 pixels
	<img src="@Url.Gravatar("my@email.com", 100, DefaultGravatar.Wavatar, GravatarRating.G)" alt="" /> // Displays a 100x100 generated Wavatar if the Gravatar does not exist as a G-rated image
	<img src="@Url.GravatarGenerator("my@email.com")..Size(100).DefaultImage(DefaultGravatar.Wavatar).Rating(GravatarRating.G)" alt="" /> // Same as above
	<img src="@Url.GravatarGenerator("my@email.com")..Size(100).DefaultImage(DefaultGravatar.Wavatar).Rating(GravatarRating.G).AppendFileType()" alt="" /> // Same as above but appends .jpg to the image name