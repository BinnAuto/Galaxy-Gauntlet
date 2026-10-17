using System.Collections.Generic;
using System.Linq;

public partial class MusicPlayer : Node
{
	[Export]
	private string SourceFolder = "./custom/music";

	[Export]
	private bool IncludeSharedFiles = true;


	private AudioStreamPlayer2D Music;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Music = (AudioStreamPlayer2D)GetNode(nameof(Music));
		Music.Finished += LoadMusic;
		LoadMusic();
	}


	private void LoadMusic()
	{
		if (false == Directory.Exists("./custom")
			|| false == Directory.Exists("./custom/music"))
		{
			return;
		}

		Directory.CreateDirectory(SourceFolder);
		string audioFile = GetRandomMusicFile();
		if (string.IsNullOrEmpty(audioFile))
		{
			return;
		}

		AudioStream audioStream = BuildAudioStream(audioFile);
		Music.Stream = audioStream;
		Music.Play();
	}


	private string GetRandomMusicFile()
	{
		// TODO: Add support for WAV and OGG file types
		List<string> files = Directory.GetFiles(SourceFolder, "*.mp3").ToList();
		if (IncludeSharedFiles)
		{
			var sharedFiles = Directory.GetFiles("./custom/music", "*.mp3");
			files.AddRange(sharedFiles);
		}
		if (files.Count == 0)
		{
			return string.Empty;
		}

		RandomNumberGenerator rng = new();
		int songIndex = rng.RandiRange(0, files.Count - 1);
		string result = files[songIndex];
		return result;
	}


	private static AudioStream BuildAudioStream(string filePath)
	{
		string extension = Path.GetExtension(filePath).ToLower();
		byte[] fileBytes = File.ReadAllBytes(filePath);
		switch(extension)
		{
			case ".mp3":
				return new AudioStreamMP3() { Data = fileBytes };

			case ".wav":
				return new AudioStreamWav() { Data = fileBytes };

			case ".ogg":
				return new AudioStreamOggVorbis() { ResourcePath = filePath };

			default:
				return null;
		}
	}
}
