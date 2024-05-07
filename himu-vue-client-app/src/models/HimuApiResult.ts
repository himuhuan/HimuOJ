// HimuApiResponseCode
export enum HimuApiCode {
    Succeed,
    UnexpectedError,
    BadRequest,
    BadAuthentication,
    LockedUser,
    ResourceNotExist,
    UpdateConcurrencyConflict,
    DuplicateItem,
    OutOfLimit
}

// HimuApiResponse
export class HimuApiResult {
    code: HimuApiCode;
    message: string;

    constructor(code: HimuApiCode, message: string) {
        this.code = code;
        this.message = message;
    }

    success(): boolean {
        return this.code === 0;
    }
}

// HimuApiResponse<T>
export class HimuApiResultWithData extends HimuApiResult {
    value: any;

    constructor(code: number, message: string, data: any) {
        super(code, message);
        this.value = data;
    }
}


