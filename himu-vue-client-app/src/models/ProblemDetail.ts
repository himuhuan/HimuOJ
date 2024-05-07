export default class ProblemDetail {
	code: string;
	title: string;
	content: string;
	maxMemoryLimitByte: number;
	maxExecuteTimeLimit: number;
	allowDownloadInput: boolean;
	allowDownloadAnswer: boolean;

	constructor(
		code: string,
		title: string,
		content: string,
		maxMemoryLimitByte: number,
		maxExecuteTimeLimit: number,
		allowDownloadInput: boolean,
		allowDownloadAnswer: boolean
	) {
		this.code = code;
		this.title = title;
		this.content = content;
		this.maxMemoryLimitByte = maxMemoryLimitByte;
		this.maxExecuteTimeLimit = maxExecuteTimeLimit;
		this.allowDownloadAnswer = allowDownloadAnswer;
		this.allowDownloadInput = allowDownloadInput;
	}
}
