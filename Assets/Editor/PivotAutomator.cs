using UnityEditor;
using UnityEngine;

public class PivotAutomator : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;

        if (textureImporter.textureType == TextureImporterType.Sprite)
        {
            TextureImporterSettings settings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(settings);

            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2(0.5f, 0f);

            textureImporter.SetTextureSettings(settings);
        }
    }
}