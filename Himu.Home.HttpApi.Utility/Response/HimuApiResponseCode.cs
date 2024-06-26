﻿namespace Himu.HttpApi.Utility
{
    public enum HimuApiResponseCode
    {
        Succeed,

        /// <summary>
        /// UnexpectedError means unforeseen results, often errors that logically hardly occur.
        /// If such an error occurs, it is most likely an error in the code itself
        /// </summary>
        UnexpectedError,

        BadRequest,
        BadAuthentication,
        LockedUser,
        ResourceNotExist,
        UpdateConcurrencyConflict,
        DuplicateItem,
        
        /// <summary>
        /// OutOfLimit means that some object in request exceeds the limit.
        /// </summary>
        OutOfLimit
    }
}