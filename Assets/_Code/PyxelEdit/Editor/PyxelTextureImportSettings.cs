using UnityEditor;
using UnityEngine;
using System;

namespace PyxelEdit
{
	[Serializable]
	public class PyxelTextureImportSettings
	{
		public bool seamlessCubemap;

		//     Mip map bias of the texture.
		public float mipmapBias = 0.5f;

		//     Texture coordinate wrapping mode.
		public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

		//     Texture U coordinate wrapping mode.
		public TextureWrapMode wrapModeU;

		//     Texture V coordinate wrapping mode.
		public TextureWrapMode wrapModeV;

		//     Texture W coordinate wrapping mode for Texture3D.
		public TextureWrapMode wrapModeW;

		//     If the provided alpha channel is transparency, enable this to dilate the color
		//     to avoid filtering artifacts on the edges.
		public bool alphaIsTransparency = true;

		//     Sprite texture import mode.
		public int spriteMode = (int)SpriteImportMode.Single;

		//     The number of pixels in the sprite that correspond to one unit in world space.
		public float spritePixelsPerUnit = 100;

		//     The tessellation detail to be used for generating the mesh for the associated
		//     sprite if the SpriteMode is set to Single. For Multiple sprites, use the SpriteEditor
		//     to specify the value per sprite. Valid values are in the range [0-1], with higher
		//     values generating a tighter mesh. A default of -1 will allow Unity to determine
		//     the value automatically.
		public float spriteTessellationDetail;

		//     The number of blank pixels to leave between the edge of the graphic and the mesh.
		public uint spriteExtrude = 1;

		//     SpriteMeshType defines the type of Mesh that TextureImporter generates for a
		//     Sprite.
		public SpriteMeshType spriteMeshType = SpriteMeshType.Tight;

		//     Edge-relative alignment of the sprite graphic.
		public int spriteAlignment = (int)SpriteAlignment.Center;

		//     Pivot point of the Sprite relative to its graphic's rectangle.
		public Vector2 spritePivot = Vector2.zero;

		//     Border sizes of the generated sprites.
		public Vector4 spriteBorder = Vector4.zero;

		//     Generates a default physics shape for a Sprite if a physics shape has not been
		//     set by the user.
		public bool spriteGenerateFallbackPhysicsShape = true;


		//     Anisotropic filtering level of the texture.
		public int aniso = 1;

		//     Filtering mode of the texture.
		public FilterMode filterMode = FilterMode.Point;

		//     Convolution mode.
		public TextureImporterCubemapConvolution cubemapConvolution;

		//     Which type of texture are we dealing with here.
		public TextureImporterType textureType = TextureImporterType.Sprite;

		//     Shape of imported texture.
		public TextureImporterShape textureShape = TextureImporterShape.Texture2D;

		//     Mipmap filtering mode.
		public TextureImporterMipFilter mipmapFilter;

		//     Generate mip maps for the texture?
		public bool mipmapEnabled = false;

		//     Is texture storing color data?
		public bool sRGBTexture = true;

		//     Fade out mip levels to gray color?
		public bool fadeOut = false;

		//     Enable this to avoid colors seeping out to the edge of the lower Mip levels.
		//     Used for light cookies.
		public bool borderMipmap;

		//     Enables or disables coverage-preserving alpha MIP mapping.
		public bool mipMapsPreserveCoverage;

		//     Mip level where texture begins to fade out to gray.
		public int mipmapFadeDistanceStart = 1;

		//     Returns or assigns the alpha test reference value.
		public float alphaTestReferenceValue = 1f;

		//     Convert heightmap to normal map?
		public bool convertToNormalMap;

		//     Amount of bumpyness in the heightmap.
		public float heightmapScale;

		//     Normal map filtering mode.
		public TextureImporterNormalFilter normalMapFilter;

		//     Select how the alpha of the imported texture is generated.
		public TextureImporterAlphaSource alphaSource = TextureImporterAlphaSource.FromInput;

		//     Color or Alpha component TextureImporterType|Single Channel Textures uses.
		public TextureImporterSingleChannelComponent singleChannelComponent;

		//     Is texture data readable from scripts.
		public bool readable = false;

		//     Enable mipmap streaming for this texture.
		public bool streamingMipmaps;

		//     Relative priority for this texture when reducing memory size in order to hit
		//     the memory budget.
		public int streamingMipmapsPriority;

		//     Scaling mode for non power of two textures.
		public TextureImporterNPOTScale npotScale;

		//     Cubemap generation mode.
		public TextureImporterGenerateCubemap generateCubemap;

		//     Mip level where texture is faded out to gray completely.
		public int mipmapFadeDistanceEnd = 3;

	}
}
