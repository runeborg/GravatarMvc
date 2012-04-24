#GravatarMvc#

> Gravatar wrapper for ASP.NET MVC. Feel free to use it any way you want.

###Usage###
	<img src="@Url.Gravatar("my@email.com", 100)" alt="" /> <!-- Displays a Gravatar for my@email.com with a size of 100x100 pixels -->
	<img src="@Url.Gravatar("my@email.com", 100, DefaultGravatar.Wavatar, GravatarRating.G)" alt="" /> <!-- Displays a 100x100 generated Wavatar if the Gravatar does not exist as a G-rated image -->
	<img src="@Url.GravatarGenerator("my@email.com").Size(100).DefaultImage(DefaultGravatar.Wavatar).Rating(GravatarRating.G).Url" alt="" /> <!-- Same as above -->
	<img src="@Url.GravatarGenerator("my@email.com").Size(100).DefaultImage(DefaultGravatar.Wavatar).Rating(GravatarRating.G).AppendFileType().Url" alt="" /> <!-- Same as above but appends .jpg to the image name -->