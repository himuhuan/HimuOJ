export interface ResourceUsage {
    memoryByteUsed: number;
    timeUsed: number;
}

export interface OutputDifference {
    expected: string;
    actual: string;
    position: number;
}

export interface ExitCodeWithMessage {
    exitCode: number;
    message: string;
}

export interface CompilerInfo {
	compilerName: string;
	messageFromCompiler: string | null;
}

export enum ExecutionStatus {
    PENDING = "PENDING",
    ACCEPTED = "ACCEPTED",
    RUNNING = "RUNNING",
    WRONG_ANSWER = "WRONG_ANSWER",
    RUNTIME_ERROR = "RUNTIME_ERROR",
    SKIPPED = "SKIPPED",
    TIME_LIMIT_EXCEEDED = "TIME_LIMIT_EXCEEDED",
    MEMORY_LIMIT_EXCEEDED = "MEMORY_LIMIT_EXCEEDED",
    COMPILE_ERROR = "COMPILE_ERROR",
    SYSTEM_ERROR = "SYSTEM_ERROR"
}

export interface PointRunResult {
    id: number;
    testStatus: string;
    
    /**
     * @field The usage of resources during the test point execution.
     */
    usage: ResourceUsage | null;

    /**
     * @field The system exit code and message from the test point execution.
     */
    runResult: ExitCodeWithMessage | null;

    testPointId: number;
    
    commitId: number;
    
    /**
     * @field The difference between the expected and actual output.
     * This field is only present if the test point is WRONG_ANSWER.
     */
    difference: OutputDifference | null;
}