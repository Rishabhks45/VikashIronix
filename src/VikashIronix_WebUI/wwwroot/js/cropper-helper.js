// ===== Image Cropping Functions for CropperJS =====

let cropper = null;

if (typeof window.showCropper !== 'function') {
    window.showCropper = function (imageSrc) {
        try {
            if (cropper) {
                cropper.destroy();
                cropper = null;
            }
            const imageElement = document.getElementById('imageToCrop');
            if (!imageElement) {
                console.error('Image element not found');
                return;
            }
            imageElement.src = imageSrc;
            setTimeout(() => {
                cropper = new Cropper(imageElement, {
                    aspectRatio: 1,
                    viewMode: 1,
                    dragMode: 'move',
                    autoCropArea: 0.8,
                    restore: false,
                    guides: true,
                    center: true,
                    highlight: true,
                    cropBoxMovable: true,
                    cropBoxResizable: true,
                    toggleDragModeOnDblclick: false,
                    ready: function () {
                        cropper.resize();
                    }
                });
            }, 300);
        } catch (error) {
            console.error('Error initializing cropper:', error);
        }
    };
}

if (typeof window.getCroppedImageBase64 !== 'function') {
    window.getCroppedImageBase64 = function () {
        return new Promise((resolve, reject) => {
            try {
                if (!cropper) {
                    console.error("Cropper not initialized");
                    return reject("Cropper not initialized");
                }

                const canvas = cropper.getCroppedCanvas({
                    width: 300,
                    height: 300,
                    imageSmoothingEnabled: true,
                    imageSmoothingQuality: 'high'
                });

                if (!canvas) {
                    console.error("Could not create cropped canvas");
                    return reject("Could not create cropped canvas");
                }

                setTimeout(() => {
                    try {
                        const result = canvas.toDataURL('image/jpeg', 0.85); // 85% quality
                        console.log("Cropped image size:", result.length);
                        resolve(result);
                    } catch (e) {
                        console.error("Error in toDataURL:", e);
                        reject("Error in toDataURL");
                    }
                }, 100); // Small delay
            } catch (e) {
                console.error("Error in getCroppedImageBase64:", e);
                reject("Error in getCroppedImageBase64");
            }
        });
    };
}

if (typeof window.destroyCropper !== 'function') {
    window.destroyCropper = function () {
        if (cropper) {
            cropper.destroy();
            cropper = null;
            // Clear the image source
            const imageElement = document.getElementById('imageToCrop');
            if (imageElement) {
                imageElement.src = "";
            }
            console.log('Cropper destroyed');
        }
    };
}

if (typeof window.compressAndReturnImageBase64 !== 'function') {
    window.compressAndReturnImageBase64 = async (inputFileElementId) => {
        const fileInput = document.getElementById(inputFileElementId);
        if (!fileInput || fileInput.files.length === 0) return null;

        const imgToCrop = document.getElementById("imageToCrop");
        if (imgToCrop) imgToCrop.src = "";

        const file = fileInput.files[0];
        const img = new Image();

        const reader = new FileReader();
        const imageLoadPromise = new Promise((resolve, reject) => {
            reader.onload = (e) => {
                img.onload = () => resolve();
                img.onerror = reject;
                img.src = e.target.result;
            };
            reader.onerror = reject;
        });

        reader.readAsDataURL(file);
        await imageLoadPromise;

        // Resize logic
        const MAX_SIZE = 1024;
        const scale = Math.min(MAX_SIZE / img.width, MAX_SIZE / img.height, 1);
        const canvas = document.createElement("canvas");
        canvas.width = img.width * scale;
        canvas.height = img.height * scale;

        const ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

        // Export as JPEG with 70% quality
        const base64 = canvas.toDataURL("image/jpeg", 0.7); // 70% quality
        return base64;
    };
}
