using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMOD
{
    public class SoundSystem
    {
        private readonly FMOD.System _system;
        public Channel ChannelBGM = null;
        public Channel ChannelBackgroundSFX = null;
        public ChannelGroup ChannelGroupSFX;
        public FMODSound sndActiveBGM;
        public string sndActiveBGMName;
        public static bool AudioFound;

        public SoundSystem()
        {
            AudioFound = true;
            sndActiveBGMName = "";

            RESULT result = Factory.System_Create(ref _system);
            if (result != RESULT.OK)
            {
                AudioFound = false;
                return;
            }
            uint version = 0;
            result = System.getVersion(ref version);
            if (result != RESULT.OK || version < VERSION.number)
            {
                AudioFound = false;
                return;
            }

            result = System.init(16, INITFLAGS.NORMAL, (IntPtr)null);
            if (result != RESULT.OK)
            {
                AudioFound = false;
                return;
            }

            ChannelBGM = new Channel();
            ChannelBGM.setRaw(new IntPtr());

            ChannelBackgroundSFX = new Channel();
            ChannelBackgroundSFX.setRaw(new IntPtr());

            ChannelGroupSFX = new ChannelGroup();
        }

        public FMOD.System System
        {
            get { return _system; }
        }

        public void ChangeBGMVolume(float Volume)
        {
            if (!SoundSystem.AudioFound)
                return;

            RESULT result = ChannelBGM.setPaused(true);
            result = ChannelBGM.setVolume(Volume);
            result = ChannelBGM.setPaused(false);
        }

        public UInt32 GetPosition(FMODSound ActiveSound)
        {
            if (ActiveSound == null || !SoundSystem.AudioFound)
                return 0;

            UInt32 Position = 0;
            ActiveSound.ActiveChannel.getPosition(ref Position, TIMEUNIT.MS);
            return Position;
        }

        public static void ReleaseSound(FMODSound ActiveSound)
        {
            if (ActiveSound != null)
                ActiveSound.Release();
        }
    }

    public class FMODSound
    {
        internal Channel ActiveChannel;
        private readonly SoundSystem _system;
        private readonly FMOD.Sound _sound;

        public FMODSound(SoundSystem system, string path)
        {
            if (!SoundSystem.AudioFound)
                return;

            _system = system;
            RESULT result = system.System.createSound(path, MODE.HARDWARE, ref _sound);
            if (result != RESULT.OK)
            {
                result = system.System.createSound(path, MODE.SOFTWARE, ref _sound);
                if (result != RESULT.OK)
                    throw new Exception("Create Sound Failed\r\n" + path + "\r\n" + result.ToString());
            }
        }

        public void Play(float Volume = 1f)
        {
            if (!SoundSystem.AudioFound)
                return;
            
            RESULT result = _system.System.playSound(CHANNELINDEX.FREE, _sound, false, ref ActiveChannel);
            if (result != RESULT.OK)
                throw new Exception("Play Sound Failed\r\n" + result.ToString());
            
            ActiveChannel.setVolume(Volume);
            ActiveChannel.setChannelGroup(_system.ChannelGroupSFX);
        }

        public void PlayAsBGM()
        {
            if (!SoundSystem.AudioFound)
                return;

            RESULT result = _system.System.playSound(CHANNELINDEX.REUSE, _sound, false, ref _system.ChannelBGM);
            ActiveChannel = _system.ChannelBGM;
            _system.sndActiveBGM = this;
            if (result != RESULT.OK)
                throw new Exception("Play Sound Failed\r\n" + result.ToString());
        }

        public void PlayAsBackgroundSFX()
        {
            if (!SoundSystem.AudioFound)
                return;

            RESULT result = _system.System.playSound(CHANNELINDEX.REUSE, _sound, false, ref _system.ChannelBackgroundSFX);
            ActiveChannel = _system.ChannelBackgroundSFX;

            if (result != RESULT.OK)
                throw new Exception("Play Sound Failed\r\n" + result.ToString());
        }

        public void SetLoop(bool Loop)
        {
            if (!SoundSystem.AudioFound)
                return;

            RESULT result = RESULT.OK;
            if (Loop)
                result = _sound.setMode(FMOD.MODE.LOOP_NORMAL);
            else
                result = _sound.setMode(FMOD.MODE.LOOP_OFF);

            if (result != RESULT.OK)
                throw new Exception("Loop Count Set Failed\r\n" + result.ToString());
        }

        public void SetPosition(UInt32 Position)
        {
            if (!SoundSystem.AudioFound)
                return;

            ActiveChannel.setPosition(Position, TIMEUNIT.MS);
        }

        public void Stop()
        {
            if (!SoundSystem.AudioFound)
                return;

            if (!IsPlaying())
                return;

            RESULT result = ActiveChannel.stop();
            if (result != RESULT.OK)
                throw new Exception("Stop Sound Failed\r\n" + result.ToString());
        }

        public bool IsPlaying()
        {
            if (!SoundSystem.AudioFound)
                return false;

            if (ActiveChannel == null)
                return false;

            bool IsPlaying = false;

            RESULT result = ActiveChannel.isPlaying(ref IsPlaying);

            return IsPlaying;
        }

        internal void Release()
        {
            if (!SoundSystem.AudioFound)
                return;

            RESULT result = _sound.release();
        }
    }
}
