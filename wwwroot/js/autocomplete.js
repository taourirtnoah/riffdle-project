(function () {
	function closeMenu(control) {
		const results = control.querySelector("[data-autocomplete-results]");
		if (!results) {
			return;
		}

		results.innerHTML = "";
		results.hidden = true;
	}

	function openMenu(control) {
		const results = control.querySelector("[data-autocomplete-results]");
		if (results) {
			results.hidden = false;
		}
	}

	function renderResults(control, items) {
		const results = control.querySelector("[data-autocomplete-results]");
		if (!results) {
			return;
		}

		results.innerHTML = "";
		if (!Array.isArray(items) || items.length === 0) {
			closeMenu(control);
			return;
		}

		const fragment = document.createDocumentFragment();
		items.forEach((item) => {
			const button = document.createElement("button");
			button.type = "button";
			button.className = "autocomplete-result";
			button.textContent = item.text;
			button.addEventListener("mousedown", (event) => {
				event.preventDefault();
				const input = control.querySelector("input[type='text']");
				const hiddenId = input?.dataset.autocompleteHiddenId;
				if (input) {
					input.value = item.text;
				}
				if (hiddenId) {
					const hidden = document.getElementById(hiddenId);
					if (hidden) {
						hidden.value = item.id;
					}
				}
				closeMenu(control);
			});
			fragment.appendChild(button);
		});

		results.appendChild(fragment);
		openMenu(control);
	}

	async function search(control, query) {
		const input = control.querySelector("input[type='text']");
		const url = input?.dataset.autocompleteUrl;
		if (!url) {
			return;
		}

		const response = await fetch(`${url}?q=${encodeURIComponent(query)}`, {
			headers: {
				Accept: "application/json"
			}
		});

		if (!response.ok) {
			throw new Error("Autocomplete request failed.");
		}

		return await response.json();
	}

	document.addEventListener("DOMContentLoaded", () => {
		document.querySelectorAll("[data-autocomplete-control]").forEach((control) => {
			const input = control.querySelector("input[type='text']");
			const hiddenId = input?.dataset.autocompleteHiddenId;
			const hidden = hiddenId ? document.getElementById(hiddenId) : null;
			let debounceHandle = null;

			if (!input) {
				return;
			}

			input.addEventListener("input", () => {
				if (hidden) {
					hidden.value = "";
				}

				window.clearTimeout(debounceHandle);
				const query = input.value.trim();
				if (query.length < 2) {
					closeMenu(control);
					return;
				}

				debounceHandle = window.setTimeout(async () => {
					try {
						const items = await search(control, query);
						renderResults(control, items);
					} catch (error) {
						console.error(error);
						closeMenu(control);
					}
				}, 250);
			});

			input.addEventListener("blur", () => {
				window.setTimeout(() => closeMenu(control), 150);
			});
		});
	});
})();
