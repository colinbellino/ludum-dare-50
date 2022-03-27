using FMODUnity;
using UnityEngine;

namespace Game.Core
{
	public static class AudioHelpers
	{
		public static void StopEverything()
		{
			FMODUnity.RuntimeManager.PauseAllEvents(true);
		}

		public static void SetVolume(string busPath, float volume)
		{
			var bus = FMODUnity.RuntimeManager.GetBus(busPath);
			bus.setVolume(volume);
		}

		public static void PlayOneShotRandom(EventReference[] eventReferences, Vector3 position = new Vector3())
		{
			var eventReference = eventReferences[UnityEngine.Random.Range(0, eventReferences.Length)];
			PlayOneShot(eventReference, position);
		}

		public static void PlayOneShot(EventReference eventReference, Vector3 position = new Vector3())
		{
			if (eventReference.IsNull)
				return;

			try
			{
				PlayOneShot(eventReference.Guid, position);
			}
			catch (EventNotFoundException)
			{
				// RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + eventReference);
			}
		}

		public static void PlayOneShot(string path, Vector3 position = new Vector3())
		{
			try
			{
				PlayOneShot(FMODUnity.RuntimeManager.PathToGUID(path), position);
			}
			catch (EventNotFoundException)
			{
				// RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
			}
		}

		public static void PlayOneShot(FMOD.GUID guid, Vector3 position = new Vector3())
		{
			var instance = RuntimeManager.CreateInstance(guid);
			instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
			instance.setPitch(Time.timeScale);
			instance.start();
			instance.release();
		}
	}
}

