using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Collections;
using System;
using System.Runtime.InteropServices;

namespace PyxelEdit
{
	[ScriptedImporter(1, "pyxel")]
	public class PyxelImporter : ScriptedImporter/* , ISpriteEditorDataProvider */
	{
		[SerializeField] private PyxelTextureImportSettings _importSettings;
		[SerializeField] [ReadOnly] public PyxelDocData _data;

		public override void OnImportAsset(AssetImportContext ctx)
		{
			UnityEngine.Debug.Log("OnImportAsset");
			var entries = new Dictionary<string, byte[]>();
			var layers = new List<string>();
			var tiles = new List<string>();
			var archiveName = Path.GetFileNameWithoutExtension(ctx.assetPath);

			using (var archive = ZipFile.Open(ctx.assetPath, ZipArchiveMode.Read))
			{
				foreach (var entry in archive.Entries)
				{
					// UnityEngine.Debug.Log(entry.FullName);

					if (entry.FullName.StartsWith("layer"))
						layers.Add(entry.FullName);

					if (entry.FullName.StartsWith("tile"))
						tiles.Add(entry.FullName);

					var stream = entry.Open();
					var memoryStream = new MemoryStream();
					stream.CopyTo(memoryStream);
					stream.Close();

					entries.Add(entry.FullName, memoryStream.GetBuffer());
				}
			}

			{
				var filename = "docData.json";
				var dataString = Encoding.UTF8.GetString(entries[filename]);
				_data = JsonConvert.DeserializeObject<PyxelDocData>(dataString);

				var text = new TextAsset(dataString);
				text.name = filename;

				ctx.AddObjectToAsset(filename, text);
			}

			// Generate preview
			{
				var previewPixels = new Color32[0];

				foreach (var filename in layers)
				{
					var texture = new Texture2D(2, 2);

					texture.LoadImage(entries[filename]);
					var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2), texture.width);
					sprite.name = archiveName + "_" + filename;

					ctx.AddObjectToAsset(filename, sprite, sprite.texture);

					var pixels = texture.GetPixels32();
					if (previewPixels.Length == 0)
					{
						previewPixels = pixels;
						continue;
					}

					for (int i = 0; i < pixels.Length; i++)
						previewPixels[i] = Color.Lerp(pixels[i], previewPixels[i], previewPixels[i].a / 1.0f);
				}

				{
					var filename = "Preview";
					var texture = new Texture2D(_data.canvas.width, _data.canvas.height);
					texture.name = filename;
					texture.SetPixels32(previewPixels);

					ctx.AddObjectToAsset(filename, texture, texture);
					ctx.SetMainObject(texture);
				}
			}

			// for (int i = 0; i < tiles.Count; i++)
			// {
			// 	var filename = tiles[i];

			// 	var texture = new Texture2D(_data.canvas.tileWidth, _data.canvas.tileHeight);
			// 	texture.LoadImage(entries[filename]);
			// 	texture.name = archiveName + "_texture_" + i;
			// 	texture.hideFlags = HideFlags.HideInHierarchy;
			// 	ctx.AddObjectToAsset(filename, texture, texture);

			// 	var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
			// 	sprite.name = archiveName + "_tile_" + i;

			// 	ctx.AddObjectToAsset(filename, sprite, sprite.texture);
			// }

			{
				var textureImporterSettings = new TextureImporterSettings()
				{
					seamlessCubemap = _importSettings.seamlessCubemap,
					mipmapBias = _importSettings.mipmapBias,
					wrapMode = _importSettings.wrapMode,
					wrapModeU = _importSettings.wrapModeU,
					wrapModeV = _importSettings.wrapModeV,
					wrapModeW = _importSettings.wrapModeW,
					alphaIsTransparency = _importSettings.alphaIsTransparency,
					spriteMode = _importSettings.spriteMode,
					spritePixelsPerUnit = _importSettings.spritePixelsPerUnit,
					spriteTessellationDetail = _importSettings.spriteTessellationDetail,
					spriteExtrude = _importSettings.spriteExtrude,
					spriteMeshType = _importSettings.spriteMeshType,
					spriteAlignment = _importSettings.spriteAlignment,
					spritePivot = _importSettings.spritePivot,
					spriteBorder = _importSettings.spriteBorder,
					spriteGenerateFallbackPhysicsShape = _importSettings.spriteGenerateFallbackPhysicsShape,
					aniso = _importSettings.aniso,
					filterMode = _importSettings.filterMode,
					cubemapConvolution = _importSettings.cubemapConvolution,
					textureType = _importSettings.textureType,
					textureShape = _importSettings.textureShape,
					mipmapFilter = _importSettings.mipmapFilter,
					mipmapEnabled = _importSettings.mipmapEnabled,
					sRGBTexture = _importSettings.sRGBTexture,
					fadeOut = _importSettings.fadeOut,
					borderMipmap = _importSettings.borderMipmap,
					mipMapsPreserveCoverage = _importSettings.mipMapsPreserveCoverage,
					mipmapFadeDistanceStart = _importSettings.mipmapFadeDistanceStart,
					alphaTestReferenceValue = _importSettings.alphaTestReferenceValue,
					convertToNormalMap = _importSettings.convertToNormalMap,
					heightmapScale = _importSettings.heightmapScale,
					normalMapFilter = _importSettings.normalMapFilter,
					alphaSource = _importSettings.alphaSource,
					singleChannelComponent = _importSettings.singleChannelComponent,
					readable = _importSettings.readable,
					streamingMipmaps = _importSettings.streamingMipmaps,
					streamingMipmapsPriority = _importSettings.streamingMipmapsPriority,
					npotScale = _importSettings.npotScale,
					generateCubemap = _importSettings.generateCubemap,
					mipmapFadeDistanceEnd = _importSettings.mipmapFadeDistanceEnd,
				};
				for (int i = 0; i < tiles.Count; i++)
				{
					var filename = tiles[i];

					var texture = new Texture2D(_data.canvas.tileWidth, _data.canvas.tileHeight);
					texture.LoadImage(entries[filename]);

					var textureInformation = new SourceTextureInformation()
					{
						containsAlpha = true,
						hdr = false,
						height = _data.canvas.tileHeight,
						width = _data.canvas.tileWidth,
					};
					var platformSettings = new TextureImporterPlatformSettings()
					{
						overridden = false,
					};
					var settings = new TextureGenerationSettings
					{
						assetPath = archiveName + "_texture_" + i,
						spriteImportData = new SpriteImportData[] {
							new SpriteImportData
							{
								alignment = (SpriteAlignment)textureImporterSettings.spriteAlignment,
								border = textureImporterSettings.spriteBorder,
								name = archiveName + "_tile_" + i,
								pivot = new Vector2(0.5f, 0.5f),
								rect = new Rect(0, 0, _data.canvas.tileWidth, _data.canvas.tileHeight),
								spriteID = GUID.Generate().ToString(),
							}
						},
						textureImporterSettings = textureImporterSettings,
						enablePostProcessor = false,
						sourceTextureInformation = textureInformation,
						qualifyForSpritePacking = true,
						platformSettings = platformSettings,
						spritePackingTag = "pyxel",
						secondarySpriteTextures = new SecondarySpriteTexture[0],
					};
					var pixels = new NativeArray<Color32>(texture.GetPixels32(), Allocator.Temp);
					var output = TextureGenerator.GenerateTexture(settings, pixels);

					output.texture.hideFlags = HideFlags.HideInHierarchy;
					ctx.AddObjectToAsset(output.texture.name, output.texture, output.texture);

					DestroyImmediate(texture);

					foreach (var sprite in output.sprites)
					{
						ctx.AddObjectToAsset(sprite.name, sprite, sprite.texture);
					}
				}
			}

			AssetDatabase.WriteImportSettingsIfDirty(assetPath);

			AssetDatabase.Refresh();
			AssetDatabase.LoadAllAssetsAtPath(assetPath);
			EditorApplication.RepaintProjectWindow();
		}
	}

	public class ReadOnlyAttribute : PropertyAttribute { }

	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}

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
