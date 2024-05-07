import { defineStore } from "pinia";
import { ProblemListItem } from "@/models/ProblemListItem.ts";
import { ProblemDetail } from "@/models/ProblemDetail";

export const useAppStore = defineStore("himuoj", {
	persist: false,
	state: () => {
		return {
			_contestLoaded: false,
			_problemLoaded: false,
			_problemIndex: 0,
			_problemListInfo: [] as ProblemListItem[],
			_problemDetail: null as ProblemDetail | null,
		};
	},
	getters: {
		contestLoaded(): boolean {
			return this._contestLoaded;
		},
		currentContestCode() : string {
			return this._problemListInfo[this._problemIndex].contestCode;
		},
		currentProblemCode() : string {
			if (this._contestLoaded) return this._problemListInfo[this._problemIndex].problemCode;
			throw new Error("Contest not loaded");
		},
		problemListInfo(): ProblemListItem[] {
			return this._problemListInfo;
		},
		problemLoaded(): boolean {
			return this._problemLoaded;
		},
		problemIndex(): number {
			return this._problemIndex;
		},
		problemDetail(): ProblemDetail | null {
			return this._problemDetail;
		},
	},
	actions: {

	},
});
