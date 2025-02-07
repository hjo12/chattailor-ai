using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using ChatTailorAI.Shared.Services.Files;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;
using ChatTailorAI.Shared.Services.Common;

namespace ChatTailorAI.Services.WinUI.FileManagement
{
    /// <summary>
    /// A class for picking and creating folders
    /// </summary>
    public class FolderService : IFolderService<StorageFolder>
    {
        private readonly IWindowHandleService _windowHandleService;

        public FolderService(IWindowHandleService windowHandleService)
        {
            _windowHandleService = windowHandleService;
        }

        /// <summary>
        /// Opens the folder picker and returns the chosen folder.
        /// </summary>
        /// <returns>Chosen storage folder, or null if none chosen.</returns>
        public async Task<StorageFolder> OpenFolderPickerAsync()
        {
            FolderPicker picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Downloads
            };

            InitializeWithWindow.Initialize(picker, _windowHandleService.GetWindowHandle());

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            return await picker.PickSingleFolderAsync();
        }

        /// <summary>
        /// Creates a new folder with its name passed as an argument inside a specified folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<StorageFolder> CreateFolderByNameAsync(StorageFolder folder, string folderName)
        {
            if (folder != null)
            {
                string todaysDate = $"{DateTime.Today.Month}-{DateTime.Today.Day}-{DateTime.Today.Year}";

                StorageFolder rootPhotoFolder =
                    await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

                StorageFolder photoFolderByDate =
                    await rootPhotoFolder.CreateFolderAsync(todaysDate, CreationCollisionOption.OpenIfExists);

                return photoFolderByDate;
            }
            return folder;
        }
    }
}