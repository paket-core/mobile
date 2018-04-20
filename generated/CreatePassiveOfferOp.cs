          // Automatically generated by xdrgen 
          // DO NOT EDIT or your changes may be overwritten

          namespace Stellar.Generated
{


// === xdr source ============================================================
//  struct CreatePassiveOfferOp
//  {
//      Asset selling; // A
//      Asset buying;  // B
//      int64 amount;  // amount taker gets. if set to 0, delete the offer
//      Price price;   // cost of A in terms of B
//  };
//  ===========================================================================
public class CreatePassiveOfferOp {
  public CreatePassiveOfferOp () {}
  public Asset Selling { get; set; }
  public Asset Buying { get; set; }
  public Int64 Amount { get; set; }
  public Price Price { get; set; }
  public static void Encode(IByteWriter stream, CreatePassiveOfferOp encodedCreatePassiveOfferOp) {
    Asset.Encode(stream, encodedCreatePassiveOfferOp.Selling);
    Asset.Encode(stream, encodedCreatePassiveOfferOp.Buying);
    Int64.Encode(stream, encodedCreatePassiveOfferOp.Amount);
    Price.Encode(stream, encodedCreatePassiveOfferOp.Price);
  }
  public static CreatePassiveOfferOp Decode(IByteReader stream) {
    CreatePassiveOfferOp decodedCreatePassiveOfferOp = new CreatePassiveOfferOp();
    decodedCreatePassiveOfferOp.Selling = Asset.Decode(stream);
    decodedCreatePassiveOfferOp.Buying = Asset.Decode(stream);
    decodedCreatePassiveOfferOp.Amount = Int64.Decode(stream);
    decodedCreatePassiveOfferOp.Price = Price.Decode(stream);
    return decodedCreatePassiveOfferOp;
  }
}
}
