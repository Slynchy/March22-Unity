using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour {

    public int speed = 3;
    Image img;
    Sprite sprite;
    Color32[] pixels;

    bool complete = false;
    
	void Awake () {
        // Copy the pixels to a new Texture2D, so we don't edit the original
        img = this.GetComponent<Image>();
        Texture2D temp = new Texture2D(img.sprite.texture.width, img.sprite.texture.height, TextureFormat.RGBA32, false);
        Color32[] oldPixels = img.sprite.texture.GetPixels32();
        pixels = temp.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = oldPixels[i];
        }
        temp.SetPixels32(pixels);
        temp.Apply();

        // Create a new sprite from this texture
        sprite = Sprite.Create(temp, new Rect(0, 0, img.sprite.texture.width, img.sprite.texture.height), new Vector2(0, 0));
        img.sprite = sprite;

        // Init alpha to be zero
        pixels = sprite.texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].a = 0;
        }
        sprite.texture.SetPixels32(pixels);
        sprite.texture.Apply();

    }
	
	void Update ()
    {
        if (complete) return;
        uint completionTimer = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            // Calculate the alpha
            int tempAlpha = (int)pixels[i].a + (speed * 5) - (int)pixels[i].r;
            // Clamp it between 0 and 255
            if (tempAlpha < 0)
            {
                tempAlpha = 0;
            }
            else if (tempAlpha > 255)
            {
                tempAlpha = 255;
                completionTimer++;
            }

            // Do the same for the color
            int tempColor = (int)pixels[i].r - speed;
            if (tempColor < 0)
            {
                tempColor = 0;
                completionTimer++;
            }
            else if (tempColor > 255)
            {
                tempColor = 255;
            }

            pixels[i].r = (byte)tempColor;
            pixels[i].g = (byte)tempColor;
            pixels[i].b = (byte)tempColor;
            pixels[i].a = (byte)tempAlpha;
        }
        sprite.texture.SetPixels32(pixels);
        sprite.texture.Apply();

        // If completionTimer has been incremented twice per pixel, it's complete
        if (completionTimer == pixels.Length * 2)
        {
            complete = true;
        }
    }
}
