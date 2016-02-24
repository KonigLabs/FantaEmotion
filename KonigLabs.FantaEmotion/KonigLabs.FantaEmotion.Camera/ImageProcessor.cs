using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EDSDKLib;
using KonigLabs.FantaEmotion.SDKData.Enums;
using KonigLabs.FantaEmotion.SDKData.Events;

namespace KonigLabs.FantaEmotion.Camera
{
    public class ImageProcessor : IDisposable
    {
        public event EventHandler<byte[]> StreamChanged;
        public event EventHandler AddCamera;
        public event EventHandler RemoveCamera;
        protected SDKHandler CameraHandler;

        public event EventHandler<CameraEventBase> CameraErrorEvent;
        List<EDSDKLib.Camera> CamList;
        bool IsInit;
        int BulbTime = 30;

        public ImageProcessor()
        {
            Cameras = new ObservableCollection<EDSDKLib.Camera>();
            CameraHandler = new SDKHandler();
        }

        public void Terminate()
        {
            Debug.WriteLine("disposing...");
            CameraHandler.ErrorEvent -= CameraHandlerOnErrorEvent;
            CameraHandler.CameraAdded -= SDK_CameraAdded;
            CameraHandler.LiveViewUpdated -= SDK_LiveViewUpdated;
            CameraHandler.ProgressChanged -= SDK_ProgressChanged;

            CameraHandler.CameraHasShutdown -= SDK_CameraHasShutdown;

            IsInit = false;
        }

        public void Initialize()
        {
            CameraHandler.Initialize();

            CameraHandler.ErrorEvent += CameraHandlerOnErrorEvent;
            CameraHandler.CameraAdded += SDK_CameraAdded;
            CameraHandler.LiveViewUpdated += SDK_LiveViewUpdated;
            CameraHandler.ProgressChanged += SDK_ProgressChanged;
            CameraHandler.CameraHasShutdown += SDK_CameraHasShutdown;
            RefreshCamera();
            IsInit = true;
        }

        public bool IsFilming()
        {
            return CameraHandler.IsFilming;
        }

        public bool StartRecordVideo(string outDirectory)
        {
            if (IsFilming())
                return false;

            CameraHandler.StartFilming(outDirectory);
            return true;
        }

        public async Task<bool> StopRecordVideo()
        {
            if (!IsFilming())
                return false;

            CameraHandler.StopFilming();
            return true;
        }

        private void CameraHandlerOnErrorEvent(object sender, ErrorEvent errorInfo)
        {
            OnErrorEvent(errorInfo);
        }

        protected virtual void OnStreamChanged(byte[] imageBuffer)
        {
            var handler = StreamChanged;
            handler?.Invoke(this, imageBuffer);
        }

        protected virtual void OnErrorEvent(ErrorEvent error)
        {
            RaiseCameraEvent(error);
        }

        protected virtual void OnShutdownEvent()
        {
            RaiseCameraEvent(new ShutdownEvent());
        }

        protected virtual void RaiseCameraEvent(CameraEventBase eventBase)
        {
            var handler = CameraErrorEvent;
            handler?.Invoke(this, eventBase);
        }

        public ObservableCollection<EDSDKLib.Camera> Cameras { get; private set; }

        public void Dispose()
        {
            if (!IsInit)
                return;

            StopRecordVideo();

            Terminate();


            CameraHandler.Dispose();
        }

        #region SDK Events

        private void SDK_ProgressChanged(int progress)
        {
            //if (progress == 100) progress = 0;
            //MainProgressBar.Value = progress;
        }

        private void SDK_LiveViewUpdated(object sender, byte[] imgBuf)
        {
            if (CameraHandler.IsLiveViewOn)
            {
                OnStreamChanged(imgBuf);
            }
        }

        private void SDK_CameraAdded()
        {
            RefreshCamera();
            if (AddCamera != null)
                AddCamera(this, new EventArgs());
        }

        private void SDK_CameraHasShutdown(object sender, EventArgs e)
        {
            //Terminate();
            CameraHandler.ShutDown();
            OnShutdownEvent();
            RefreshCamera();
            if (RemoveCamera != null)
                RemoveCamera(this, new EventArgs());
        }

        #endregion

        #region Session

        public bool DoOpenSession()
        {
            if (!CameraHandler.CameraSessionOpen)
                return OpenSession();

            //CloseSession();
            return true;
        }

        public EDSDKLib.Camera SelectedCamera { get; private set; }

        public void DoRefresh()
        {
            RefreshCamera();
        }

        public void DoTakePicture(Action<byte[]> callback)
        {
            CameraHandler.StopLiveView();

            CameraHandler.TakePhoto(callback, SelectedCamera); //todo test comment
        }

        public Task<byte[]> DoTakePicture()
        {
            CameraHandler.StopLiveView();

            var source = new TaskCompletionSource<byte[]>();

            CameraHandler.TakePhoto(result => source.TrySetResult(result), SelectedCamera); //todo test comment

            return source.Task;
        }

        #endregion

        #region Settings

        #endregion

        public void StartLiveView()
        {
            if (!CameraHandler.IsLiveViewOn)
            {
                CameraHandler.StartLiveView(SelectedCamera);
            }
            else
            {
                CameraHandler.StopLiveView();
                OnStreamChanged(null);
            }
        }


        public void SetFocus(uint focus)
        {
            CameraHandler.SetFocus(focus);
        }

        public void SetSetting(uint settingId, uint settingValue)
        {
            CameraHandler.SetSetting(settingId, settingValue, SelectedCamera);
        }

        #region Subroutines

        public void CloseSession()
        {
            CameraHandler.CloseSession();
        }

        private void RefreshCamera()
        {
            CloseSession();
            Cameras.Clear();
            foreach (var camera in CameraHandler.GetCameraList())
            {
                Cameras.Add(camera);
            }

            SelectedCamera = Cameras.FirstOrDefault();
        }

        private bool OpenSession()
        {
            if (SelectedCamera != null)
            {
                CameraHandler.OpenSession(SelectedCamera);
                return true;
            }

            return false;
        }

        #endregion
    }
}
