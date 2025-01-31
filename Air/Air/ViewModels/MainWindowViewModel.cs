using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Air.Connection;
using Air.Views;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ReactiveUI;

namespace Air.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        var connectionManager = new CallConnectionManager();
        JoinRoomPanelVM = new JoinRoomPanelViewModel(connectionManager);
        CallPanelVM = new CallPanelViewModel(connectionManager);
    }

    private const string DefaultCheckAudioButtonText = "Check Audio";
    private const string SecondaryCheckAudioButtonText = "Stop";
    
    public static Color DarkThemeBackgroundColor => new Color(255, 43, 41, 51);

    public CallPanelViewModel CallPanelVM { get; }
    public JoinRoomPanelViewModel JoinRoomPanelVM { get; }

    public ObservableCollection<AudioDeviceItemViewModel> OutputDevices { get; } = new();
    public ObservableCollection<AudioDeviceItemViewModel> InputDevices { get; } = new();
    
    [ObservableProperty] private AudioDeviceItemViewModel? _selectedOutputDevice;
    [ObservableProperty] private AudioDeviceItemViewModel? _selectedInputDevice;

    [ObservableProperty] private bool _isMonitoringAudio;
    [ObservableProperty] private string? _checkAudioButtonText = DefaultCheckAudioButtonText;
    
    private WasapiCapture? _recordingDevice;
    private DirectSoundOut? _renderDevice;

    public void SwitchAudioMonitoring()
    {
        if (!IsMonitoringAudio)
        {
            Debug.Assert(SelectedInputDevice != null); // TODO if this is null, we should make check audio button inactive
            Debug.Assert(SelectedInputDevice.InputDeviceId != null);
            var captureDevice = new MMDeviceEnumerator()
                .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
                .FirstOrDefault(device => device.ID == SelectedInputDevice.InputDeviceId);
            Debug.Assert(captureDevice != null); // TODO we should notify user about this error instead
            _recordingDevice = new WasapiCapture(captureDevice);
            _recordingDevice.RecordingStopped += (_, _) =>
            {
                _recordingDevice.Dispose();
                _recordingDevice = null;
            };
            
            Debug.Assert(SelectedOutputDevice != null); // TODO if this is null, we should make check audio button inactive
            Debug.Assert(SelectedOutputDevice.OutputDeviceGuid != Guid.Empty);
            _renderDevice = new DirectSoundOut(SelectedOutputDevice.OutputDeviceGuid);
            _renderDevice.PlaybackStopped += (_, _) =>
            {
                _renderDevice.Dispose();
                _renderDevice = null;
            };

            var recordingWaveProvider = new BufferedWaveProvider(_recordingDevice.WaveFormat);
            var monoSampleProvider = new StereoToMonoSampleProvider(recordingWaveProvider.ToSampleProvider());
            // TODO add filtering before resampling
            var resampler = new WdlResamplingSampleProvider(monoSampleProvider, 48000);
            
            _recordingDevice.DataAvailable += (_, args) =>
            {
                recordingWaveProvider.AddSamples(args.Buffer, 0, args.BytesRecorded);
            };
            _renderDevice.Init(resampler);
            
            _renderDevice.Play();
            _recordingDevice.StartRecording();
            
            IsMonitoringAudio = true;
        }
        else
        {
            StopMonitoringAudio();
        }
    }

    private void StopMonitoringAudio()
    {
        // TODO these assertions can fail if we click check audio button very fast, we should make this button inactive until all threads have stopped
        Debug.Assert(_recordingDevice != null);
        _recordingDevice.StopRecording();
        Debug.Assert(_renderDevice != null);
        _renderDevice.Stop();
        
        IsMonitoringAudio = false;
    }

    public void EnsureStopAudioMonitoring()
    {
        if (IsMonitoringAudio)
            StopMonitoringAudio();
    }
    
    partial void OnIsMonitoringAudioChanged(bool value)
    {
        CheckAudioButtonText = value ? SecondaryCheckAudioButtonText : DefaultCheckAudioButtonText;
    }
    
    public void LoadAudioDevicesInfo()
    {
        OutputDevices.Clear();
        
        var userOutputDevices = DirectSoundOut.Devices.ToArray();
        var defaultOutputDeviceViewModel = AudioDeviceItemViewModel
            .OutputDevice("Default Device", DirectSoundOut.DSDEVID_DefaultPlayback);
        defaultOutputDeviceViewModel.Command = ReactiveCommand.Create(() => SelectedOutputDevice = defaultOutputDeviceViewModel);
        OutputDevices.Add(defaultOutputDeviceViewModel);
        
        foreach (var device in userOutputDevices.Skip(1)) // skip Primary Audio Driver
        {
            var deviceViewModel = AudioDeviceItemViewModel.OutputDevice(device.Description, device.Guid);
            deviceViewModel.Command = ReactiveCommand.Create(() => SelectedOutputDevice = deviceViewModel);
            OutputDevices.Add(deviceViewModel);
        }
        
        if (SelectedOutputDevice == null
            || OutputDevices.All(item => item.OutputDeviceGuid != SelectedOutputDevice.OutputDeviceGuid))
            SelectedOutputDevice = OutputDevices.Count > 0 ? OutputDevices[0] : null;
        
        InputDevices.Clear();
        
        var enumerator = new MMDeviceEnumerator();
        var userInputDevices = enumerator
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
        var defaultInputDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
        var defaultInputDeviceViewModel = AudioDeviceItemViewModel.InputDevice("Default Device", defaultInputDevice.ID);
        defaultInputDeviceViewModel.Command = ReactiveCommand
            .Create(() => SelectedInputDevice = defaultInputDeviceViewModel);
        InputDevices.Add(defaultInputDeviceViewModel);
        
        foreach (var device in userInputDevices)
        {
            var deviceViewModel = AudioDeviceItemViewModel.InputDevice(device.FriendlyName, device.ID);
            deviceViewModel.Command = ReactiveCommand.Create(() => SelectedInputDevice = deviceViewModel);
            InputDevices.Add(deviceViewModel);
        }
        
        if (SelectedInputDevice == null
            || InputDevices.All(item => item.InputDeviceId != SelectedInputDevice.InputDeviceId))
            SelectedInputDevice = InputDevices.Count > 0 ? InputDevices[0] : null;
    }
}