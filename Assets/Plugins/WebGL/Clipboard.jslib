mergeInto(LibraryManager.library, {
  CopyToClipboard: function (textPtr) {
    const text = UTF8ToString(textPtr);

    if (navigator.clipboard && window.isSecureContext) {
      navigator.clipboard
        .writeText(text)
        .then(() => {
          console.log("Copied using clipboard API");
        })
        .catch((err) => {
          console.error(err);
        });
    } else {
      const textArea = document.createElement("textarea");
      textArea.value = text;

      // Avoid scrolling
      textArea.style.position = "fixed";
      textArea.style.left = "-999999px";
      textArea.style.top = "-999999px";

      document.body.appendChild(textArea);

      textArea.focus();
      textArea.select();

      try {
        document.execCommand("copy");
        console.log("Copied using execCommand");
      } catch (err) {
        console.error("Fallback copy failed", err);
      }

      document.body.removeChild(textArea);
    }
  },
});
