import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import path, { resolve } from "path";

export default defineConfig({
	plugins: [vue()],
	resolve: {
		alias: {
			"@": resolve(__dirname, "src"),
			"#": resolve(__dirname, "src/components"),
		},
	},
	optimizeDeps: {
		include: [
			`monaco-editor/esm/vs/language/json/json.worker`,
			`monaco-editor/esm/vs/language/css/css.worker`,
			`monaco-editor/esm/vs/language/html/html.worker`,
			`monaco-editor/esm/vs/language/typescript/ts.worker`,
			`monaco-editor/esm/vs/editor/editor.worker`,
		],
	},
});
