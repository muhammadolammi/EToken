namespace EToken.Application.Dtos;



public record ConfirmEnrolmentRequest
(
      Guid DeviceId 
);


public record RegisterRequest(
    string UserName,
    string Password,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string AccountType
);

public record LoginRequest(
    string UserName,
    string Password
);

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    string Cif
);

public record InitEnrolmentRequest(
        Guid DeviceId,

    string DeviceModel,
    Guid Cif,
    string DevicePublicKey
);

  
  public record InitEnrolmentResponse(
    Guid DeviceId ,
    string RsaEncryptedSecret
);



public record RevokeRequest
(
          Guid DeviceId 
);

public record VerifyRequest
(
         Guid DeviceId,
         string Code,
         string ActionType 
);


public record MeResponse(
      string Cif,
     string FirstName,
     string LastName,
     string UserName,
     string Email,
     string PhoneNumber

);


public record CreateAccountRequest
(
        
         string AccountType 
);

   public record AccountResponse
(
        
    string Cif,
    string Id,
    string Number,
     string Type,
     string Balance,
     string Status

);





public record NameEnquiryResponse(
    string AccountCif,
    string AccountNumber,
    string AccountName,
    string AccountType
);

public record TransferRequest(
    Guid SourceAccountId,
    string DestinationAccountNumber,
    decimal Amount,
    string Narration,
    Guid DeviceId, 
    string ETokenCode
);

public record TransferResponse(
    Guid TransactionId,
    string Reference,
    decimal Amount,
    string Status,
    DateTimeOffset Timestamp
);


public record GetDeviceResponse(
    string Status
);