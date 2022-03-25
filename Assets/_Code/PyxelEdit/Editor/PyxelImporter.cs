using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Newtonsoft.Json;

namespace PyxelEdit
{
	[ScriptedImporter(1, "pyxel")]
	public class PyxelImporter : ScriptedImporter
	{
		[SerializeField] [ReadOnly] public PyxelDocData _data;

		public override void OnImportAsset(AssetImportContext ctx)
		{
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

				foreach (var filename in layers)
				{
					var texture = new Texture2D(2, 2);
					texture.LoadImage(entries[filename]);
					var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2), texture.width);
					sprite.name = archiveName + "_" + filename;

					ctx.AddObjectToAsset(filename, sprite, sprite.texture);
				}

				// var textureInformation = new SourceTextureInformation()
				// {
				// 	containsAlpha = true,
				// 	hdr = false,
				// 	height = textureHeight,
				// 	width = textureWidth
				// };

				// var platformSettings = new TextureImporterPlatformSettings()
				// {
				// 	overridden = false
				// };
				// TextureImporterSettings textureImporterSettings = new TextureImporterSettings()
				// {
				// 	seamlessCubemap = seamlessCubemap,
				// 	mipmapBias = mipmapBias,
				// 	wrapMode = wrapMode,
				// 	wrapModeU = wrapModeU,
				// 	wrapModeV = wrapModeV,
				// 	wrapModeW = wrapModeW,
				// 	alphaIsTransparency = alphaIsTransparency,
				// 	spriteMode = spriteMode,
				// 	spritePixelsPerUnit = spritePixelsPerUnit,
				// 	spriteTessellationDetail = spriteTessellationDetail,
				// 	spriteExtrude = spriteExtrude,
				// 	spriteMeshType = spriteMeshType,
				// 	spriteAlignment = spriteAlignment,
				// 	spritePivot = spritePivot,
				// 	spriteBorder = spriteBorder,
				// 	spriteGenerateFallbackPhysicsShape = spriteGenerateFallbackPhysicsShape,
				// 	aniso = aniso,
				// 	filterMode = filterMode,
				// 	cubemapConvolution = cubemapConvolution,
				// 	textureType = textureType,
				// 	textureShape = textureShape,
				// 	mipmapFilter = mipmapFilter,
				// 	mipmapEnabled = mipmapEnabled,
				// 	sRGBTexture = sRGBTexture,
				// 	fadeOut = fadeOut,
				// 	borderMipmap = borderMipmap,
				// 	mipMapsPreserveCoverage = mipMapsPreserveCoverage,
				// 	mipmapFadeDistanceStart = mipmapFadeDistanceStart,
				// 	alphaTestReferenceValue = alphaTestReferenceValue,
				// 	convertToNormalMap = convertToNormalMap,
				// 	heightmapScale = heightmapScale,
				// 	normalMapFilter = normalMapFilter,
				// 	alphaSource = alphaSource,
				// 	singleChannelComponent = singleChannelComponent,
				// 	readable = readable,
				// 	streamingMipmaps = streamingMipmaps,
				// 	streamingMipmapsPriority = streamingMipmapsPriority,
				// 	npotScale = npotScale,
				// 	generateCubemap = generateCubemap,
				// 	mipmapFadeDistanceEnd = mipmapFadeDistanceEnd
				// };
				// var textureGenerationSettings = new TextureGenerationSettings()
				// {
				// 	assetPath = assetPath,
				// 	spriteImportData = ConvertAseFileSpriteImportDataToUnity(SpriteImportData),
				// 	textureImporterSettings = textureImporterSettings,
				// 	enablePostProcessor = false,
				// 	sourceTextureInformation = textureInformation,
				// 	qualifyForSpritePacking = true,
				// 	platformSettings = platformSettings,
				// 	spritePackingTag = "aseprite",
				// 	secondarySpriteTextures = new SecondarySpriteTexture[0]
				// };
				// var output = TextureGenerator.GenerateTexture(textureGenerationSettings, new Unity.Collections.NativeArray<Color32>(atlas.GetPixels32(), Unity.Collections.Allocator.Temp));

				foreach (var filename in tiles)
				{
					var texture = new Texture2D(2, 2);
					texture.LoadImage(entries[filename]);
					var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2), texture.width);
					sprite.name = archiveName + "_" + filename;

					ctx.AddObjectToAsset(filename, sprite, sprite.texture);
				}

				{
					var filename = "docData.json";
					var dataString = Encoding.UTF8.GetString(entries[filename]);
					_data = JsonConvert.DeserializeObject<PyxelDocData>(dataString);

					var text = new TextAsset(dataString);
					text.name = filename;

					ctx.AddObjectToAsset(filename, text);
					ctx.SetMainObject(text);
				}

			}
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
}
