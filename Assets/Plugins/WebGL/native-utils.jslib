mergeInto(LibraryManager.library, {
	bindTexture: function (texture) {
		GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
		GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, true);
		GLctx.texSubImage2D(GLctx.TEXTURE_2D, 0, 0, 0, GLctx.RGBA, GLctx.UNSIGNED_BYTE, image);
		GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, false);
	},

	exportFile: function (fileName, mimeType, data, size) {
		const bytes = new Uint8Array(size);
		for (var i = 0; i < size; i++) {
			bytes[i] = HEAPU8[data + i];
		}
		const anchor = document.createElement('a');
		const blob = new Blob([bytes], { type: UTF8ToString(mimeType) })
		anchor.setAttribute('href', URL.createObjectURL(blob));
		anchor.setAttribute('download', UTF8ToString(fileName));

		document.body.appendChild(anchor);
		anchor.click();
		document.body.removeChild(anchor);
	},

	copyToClipboard: function (text) {
		const value = UTF8ToString(text)
		if (navigator.clipboard) {
			navigator.clipboard.writeText(value).catch(() => {
				execCopy();
			});
		} else {
			execCopy();
		}

		function execCopy() {
			if (document.execCommand) {
				const input = document.createElement('input');
				input.setAttribute('type', 'text')
				input.setAttribute('value', value)

				document.body.appendChild(input);
				input.select();
				const success = document.execCommand('copy');
				document.body.removeChild(input);
				if (!success) {
					error();
				}
			} else {
				error();
			}

			function error() {
				console.error('Failed to copy to clipboard.');
			}
		}
	}
});