          // Automatically generated by xdrgen 
          // DO NOT EDIT or your changes may be overwritten

          namespace Stellar.Generated
{


// === xdr source ============================================================
//  struct SCPStatement
//  {
//      NodeID nodeID;    // v
//      uint64 slotIndex; // i
//  
//      union switch (SCPStatementType type)
//      {
//      case SCP_ST_PREPARE:
//          struct
//          {
//              Hash quorumSetHash;       // D
//              SCPBallot ballot;         // b
//              SCPBallot* prepared;      // p
//              SCPBallot* preparedPrime; // p'
//              uint32 nC;                // c.n
//              uint32 nH;                // h.n
//          } prepare;
//      case SCP_ST_CONFIRM:
//          struct
//          {
//              SCPBallot ballot;   // b
//              uint32 nPrepared;   // p.n
//              uint32 nCommit;     // c.n
//              uint32 nH;          // h.n
//              Hash quorumSetHash; // D
//          } confirm;
//      case SCP_ST_EXTERNALIZE:
//          struct
//          {
//              SCPBallot commit;         // c
//              uint32 nH;                // h.n
//              Hash commitQuorumSetHash; // D used before EXTERNALIZE
//          } externalize;
//      case SCP_ST_NOMINATE:
//          SCPNomination nominate;
//      }
//      pledges;
//  };
//  ===========================================================================
public class SCPStatement {
  public SCPStatement () {}
  public NodeID NodeID { get; set; }
  public Uint64 SlotIndex { get; set; }
  public SCPStatementPledges Pledges { get; set; }
  public static void Encode(IByteWriter stream, SCPStatement encodedSCPStatement) {
    NodeID.Encode(stream, encodedSCPStatement.NodeID);
    Uint64.Encode(stream, encodedSCPStatement.SlotIndex);
    SCPStatementPledges.Encode(stream, encodedSCPStatement.Pledges);
  }
  public static SCPStatement Decode(IByteReader stream) {
    SCPStatement decodedSCPStatement = new SCPStatement();
    decodedSCPStatement.NodeID = NodeID.Decode(stream);
    decodedSCPStatement.SlotIndex = Uint64.Decode(stream);
    decodedSCPStatement.Pledges = SCPStatementPledges.Decode(stream);
    return decodedSCPStatement;
  }

  public class SCPStatementPledges {
    public SCPStatementPledges () {}
    public SCPStatementType Discriminant { get; set; } = new SCPStatementType();
    public SCPStatementPrepare Prepare { get; set; } = default(SCPStatementPrepare);
    public SCPStatementConfirm Confirm { get; set; } = default(SCPStatementConfirm);
    public SCPStatementExternalize Externalize { get; set; } = default(SCPStatementExternalize);
    public SCPNomination Nominate { get; set; } = default(SCPNomination);
    public static void Encode(IByteWriter stream, SCPStatementPledges encodedSCPStatementPledges) {
    XdrEncoding.EncodeInt32((int)encodedSCPStatementPledges.Discriminant.InnerValue, stream);
    switch (encodedSCPStatementPledges.Discriminant.InnerValue) {
    case SCPStatementType.SCPStatementTypeEnum.SCP_ST_PREPARE:
    SCPStatementPrepare.Encode(stream, encodedSCPStatementPledges.Prepare);
    break;
    case SCPStatementType.SCPStatementTypeEnum.SCP_ST_CONFIRM:
    SCPStatementConfirm.Encode(stream, encodedSCPStatementPledges.Confirm);
    break;
    case SCPStatementType.SCPStatementTypeEnum.SCP_ST_EXTERNALIZE:
    SCPStatementExternalize.Encode(stream, encodedSCPStatementPledges.Externalize);
    break;
    case SCPStatementType.SCPStatementTypeEnum.SCP_ST_NOMINATE:
    SCPNomination.Encode(stream, encodedSCPStatementPledges.Nominate);
    break;
    }
    }
    public static SCPStatementPledges Decode(IByteReader stream) {
      SCPStatementPledges decodedSCPStatementPledges = new SCPStatementPledges();
    decodedSCPStatementPledges.Discriminant = SCPStatementType.Decode(stream);
    switch (decodedSCPStatementPledges.Discriminant.InnerValue) {
    case SCPStatementType.SCPStatementTypeEnum.SCP_ST_PREPARE:
    decodedSCPStatementPledges.Prepare = SCPStatementPrepare.Decode(stream);
    break;
    case SCPStatementType.SCPStatementTypeEnum.SCP_ST_CONFIRM:
    decodedSCPStatementPledges.Confirm = SCPStatementConfirm.Decode(stream);
    break;
    case SCPStatementType.SCPStatementTypeEnum.SCP_ST_EXTERNALIZE:
    decodedSCPStatementPledges.Externalize = SCPStatementExternalize.Decode(stream);
    break;
    case SCPStatementType.SCPStatementTypeEnum.SCP_ST_NOMINATE:
    decodedSCPStatementPledges.Nominate = SCPNomination.Decode(stream);
    break;
    }
      return decodedSCPStatementPledges;
    }

    public class SCPStatementPrepare {
      public SCPStatementPrepare () {}
      public Hash QuorumSetHash { get; set; }
      public SCPBallot Ballot { get; set; }
      public SCPBallot Prepared { get; set; }
      public SCPBallot PreparedPrime { get; set; }
      public Uint32 NC { get; set; }
      public Uint32 NH { get; set; }
      public static void Encode(IByteWriter stream, SCPStatementPrepare encodedSCPStatementPrepare) {
        Hash.Encode(stream, encodedSCPStatementPrepare.QuorumSetHash);
        SCPBallot.Encode(stream, encodedSCPStatementPrepare.Ballot);
        if (encodedSCPStatementPrepare.Prepared != null) {
        XdrEncoding.EncodeInt32(1, stream);
        SCPBallot.Encode(stream, encodedSCPStatementPrepare.Prepared);
        } else {
        XdrEncoding.EncodeInt32(0, stream);
        }
        if (encodedSCPStatementPrepare.PreparedPrime != null) {
        XdrEncoding.EncodeInt32(1, stream);
        SCPBallot.Encode(stream, encodedSCPStatementPrepare.PreparedPrime);
        } else {
        XdrEncoding.EncodeInt32(0, stream);
        }
        Uint32.Encode(stream, encodedSCPStatementPrepare.NC);
        Uint32.Encode(stream, encodedSCPStatementPrepare.NH);
      }
      public static SCPStatementPrepare Decode(IByteReader stream) {
        SCPStatementPrepare decodedSCPStatementPrepare = new SCPStatementPrepare();
        decodedSCPStatementPrepare.QuorumSetHash = Hash.Decode(stream);
        decodedSCPStatementPrepare.Ballot = SCPBallot.Decode(stream);
        int preparedPresent = XdrEncoding.DecodeInt32(stream);
        if (preparedPresent != 0) {
        decodedSCPStatementPrepare.Prepared = SCPBallot.Decode(stream);
        }
        int preparedPrimePresent = XdrEncoding.DecodeInt32(stream);
        if (preparedPrimePresent != 0) {
        decodedSCPStatementPrepare.PreparedPrime = SCPBallot.Decode(stream);
        }
        decodedSCPStatementPrepare.NC = Uint32.Decode(stream);
        decodedSCPStatementPrepare.NH = Uint32.Decode(stream);
        return decodedSCPStatementPrepare;
      }

    }
    public class SCPStatementConfirm {
      public SCPStatementConfirm () {}
      public SCPBallot Ballot { get; set; }
      public Uint32 NPrepared { get; set; }
      public Uint32 NCommit { get; set; }
      public Uint32 NH { get; set; }
      public Hash QuorumSetHash { get; set; }
      public static void Encode(IByteWriter stream, SCPStatementConfirm encodedSCPStatementConfirm) {
        SCPBallot.Encode(stream, encodedSCPStatementConfirm.Ballot);
        Uint32.Encode(stream, encodedSCPStatementConfirm.NPrepared);
        Uint32.Encode(stream, encodedSCPStatementConfirm.NCommit);
        Uint32.Encode(stream, encodedSCPStatementConfirm.NH);
        Hash.Encode(stream, encodedSCPStatementConfirm.QuorumSetHash);
      }
      public static SCPStatementConfirm Decode(IByteReader stream) {
        SCPStatementConfirm decodedSCPStatementConfirm = new SCPStatementConfirm();
        decodedSCPStatementConfirm.Ballot = SCPBallot.Decode(stream);
        decodedSCPStatementConfirm.NPrepared = Uint32.Decode(stream);
        decodedSCPStatementConfirm.NCommit = Uint32.Decode(stream);
        decodedSCPStatementConfirm.NH = Uint32.Decode(stream);
        decodedSCPStatementConfirm.QuorumSetHash = Hash.Decode(stream);
        return decodedSCPStatementConfirm;
      }

    }
    public class SCPStatementExternalize {
      public SCPStatementExternalize () {}
      public SCPBallot Commit { get; set; }
      public Uint32 NH { get; set; }
      public Hash CommitQuorumSetHash { get; set; }
      public static void Encode(IByteWriter stream, SCPStatementExternalize encodedSCPStatementExternalize) {
        SCPBallot.Encode(stream, encodedSCPStatementExternalize.Commit);
        Uint32.Encode(stream, encodedSCPStatementExternalize.NH);
        Hash.Encode(stream, encodedSCPStatementExternalize.CommitQuorumSetHash);
      }
      public static SCPStatementExternalize Decode(IByteReader stream) {
        SCPStatementExternalize decodedSCPStatementExternalize = new SCPStatementExternalize();
        decodedSCPStatementExternalize.Commit = SCPBallot.Decode(stream);
        decodedSCPStatementExternalize.NH = Uint32.Decode(stream);
        decodedSCPStatementExternalize.CommitQuorumSetHash = Hash.Decode(stream);
        return decodedSCPStatementExternalize;
      }

    }
  }
}
}
