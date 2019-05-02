using System.Linq;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Entity.Ref;

namespace WaterTreatment.Web
{

	public partial class WTContext
	{

		public readonly RefData Ref;

        public WTContext() : base("WaterTreatment")
        {
            Ref = new RefData(this);
        }

	}

}

namespace WaterTreatment.Web.Entity.Ref
{

	public class RefData
	{

		protected readonly WTContext Context;

		public ExtensionWhitelistRef Whitelist { get; private set; }
		public LandingActionRef LandingActions { get; private set; }
		public MainSectionRef MainSections { get; private set; }
		public ParameterTypeRef ParameterTypes { get; private set; }
		public ParameterRef Parameters { get; private set; }
		public SubSectionRef SubSections { get; private set; }
		public RoleRef Roles { get; private set; }
		public SystemTypeRef SystemTypes { get; private set; }
		public UserRef Users { get; private set; }
		public BuildingRef Buildings { get; private set; }
		public SiteRef Sites { get; private set; }
		public SettingRef Settings { get; private set; }

		public RefData(WTContext context)
        {
            Context = context;
			
			Whitelist = new ExtensionWhitelistRef(Context);
			LandingActions = new LandingActionRef(Context);
			MainSections = new MainSectionRef(Context);
			ParameterTypes = new ParameterTypeRef(Context);
			Parameters = new ParameterRef(Context);
			SubSections = new SubSectionRef(Context);
			Roles = new RoleRef(Context);
			SystemTypes = new SystemTypeRef(Context);
			Users = new UserRef(Context);
			Buildings = new BuildingRef(Context);
			Sites = new SiteRef(Context);
			Settings = new SettingRef(Context);
        }

	}

	#region ExtensionWhitelist Reference Data

	public class ExtensionWhitelistIds
	{
		public readonly int _pdf = 1;
		public readonly int _doc = 2;
		public readonly int _docx = 3;
		public readonly int _png = 4;
		public readonly int _jpg = 5;
		public readonly int _jpeg = 6;
		public readonly int _txt = 7;
		public readonly int _rtx = 8;
		public readonly int _gif = 9;
		public readonly int _bmp = 10;
		public readonly int _xml = 11;
	}

	public class ExtensionWhitelistRef
	{

		protected readonly WTContext Context;

		public ExtensionWhitelistIds Id { get; private set; }

        public ExtensionWhitelistRef(WTContext context)
        {            
			Context = context;
			Id = new ExtensionWhitelistIds();
        }
		
		public ExtensionWhitelist _pdf { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._pdf); } }
		public ExtensionWhitelist _doc { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._doc); } }
		public ExtensionWhitelist _docx { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._docx); } }
		public ExtensionWhitelist _png { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._png); } }
		public ExtensionWhitelist _jpg { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._jpg); } }
		public ExtensionWhitelist _jpeg { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._jpeg); } }
		public ExtensionWhitelist _txt { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._txt); } }
		public ExtensionWhitelist _rtx { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._rtx); } }
		public ExtensionWhitelist _gif { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._gif); } }
		public ExtensionWhitelist _bmp { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._bmp); } }
		public ExtensionWhitelist _xml { get { return Context.Set<ExtensionWhitelist>().First(x => x.Id == Id._xml); } }

	}

	#endregion

	#region LandingAction Reference Data

	public class LandingActionIds
	{
		public readonly int UserAdministration = 1;
		public readonly int ConfigureSites = 2;
		public readonly int ConfigureSystemTypes = 3;
		public readonly int NewDataEntryReport = 4;
		public readonly int ContinueDataEntryReport = 5;
		public readonly int ViewDataReports = 6;
	}

	public class LandingActionRef
	{

		protected readonly WTContext Context;

		public LandingActionIds Id { get; private set; }

        public LandingActionRef(WTContext context)
        {            
			Context = context;
			Id = new LandingActionIds();
        }
		
		public LandingAction UserAdministration { get { return Context.Set<LandingAction>().First(x => x.Id == Id.UserAdministration); } }
		public LandingAction ConfigureSites { get { return Context.Set<LandingAction>().First(x => x.Id == Id.ConfigureSites); } }
		public LandingAction ConfigureSystemTypes { get { return Context.Set<LandingAction>().First(x => x.Id == Id.ConfigureSystemTypes); } }
		public LandingAction NewDataEntryReport { get { return Context.Set<LandingAction>().First(x => x.Id == Id.NewDataEntryReport); } }
		public LandingAction ContinueDataEntryReport { get { return Context.Set<LandingAction>().First(x => x.Id == Id.ContinueDataEntryReport); } }
		public LandingAction ViewDataReports { get { return Context.Set<LandingAction>().First(x => x.Id == Id.ViewDataReports); } }

	}

	#endregion

	#region MainSection Reference Data

	public class MainSectionIds
	{
		public readonly int Home = 1;
		public readonly int Reports = 2;
		public readonly int DataEntry = 3;
		public readonly int ConfigureSites = 4;
		public readonly int ConfigureSystemTypes = 5;
		public readonly int UserAdministration = 6;
	}

	public class MainSectionRef
	{

		protected readonly WTContext Context;

		public MainSectionIds Id { get; private set; }

        public MainSectionRef(WTContext context)
        {            
			Context = context;
			Id = new MainSectionIds();
        }
		
		public MainSection Home { get { return Context.Set<MainSection>().First(x => x.Id == Id.Home); } }
		public MainSection Reports { get { return Context.Set<MainSection>().First(x => x.Id == Id.Reports); } }
		public MainSection DataEntry { get { return Context.Set<MainSection>().First(x => x.Id == Id.DataEntry); } }
		public MainSection ConfigureSites { get { return Context.Set<MainSection>().First(x => x.Id == Id.ConfigureSites); } }
		public MainSection ConfigureSystemTypes { get { return Context.Set<MainSection>().First(x => x.Id == Id.ConfigureSystemTypes); } }
		public MainSection UserAdministration { get { return Context.Set<MainSection>().First(x => x.Id == Id.UserAdministration); } }

	}

	#endregion

	#region ParameterType Reference Data

	public class ParameterTypeIds
	{
		public readonly int N_A = 1;
		public readonly int Number = 2;
	}

	public class ParameterTypeRef
	{

		protected readonly WTContext Context;

		public ParameterTypeIds Id { get; private set; }

        public ParameterTypeRef(WTContext context)
        {            
			Context = context;
			Id = new ParameterTypeIds();
        }
		
		public ParameterType N_A { get { return Context.Set<ParameterType>().First(x => x.Id == Id.N_A); } }
		public ParameterType Number { get { return Context.Set<ParameterType>().First(x => x.Id == Id.Number); } }

	}

	#endregion

	#region Parameter Reference Data

	public class ParameterIds
	{
		public readonly int TotalCalciumConcentrationinSupplyWatertoFacility = 1;
		public readonly int TotalMagnesiumConcentrationinSupplyWatertoFacility = 2;
		public readonly int TotalHardnessofSupplyWatertoMedicalFacility = 3;
		public readonly int TotalHardnessofSupplyWatertoMedicalFacilityPPM = 4;
		public readonly int TotalHardnessofSupplyWatertoDentalFacility = 5;
		public readonly int TotalHardnessofSupplyWatertoDentalFacilityPPM = 6;
		public readonly int TotalHardnessofSoftWatertoMedicalFacilityGPG = 7;
		public readonly int TotalHardnessofSoftWatertoDentalFacilityGPG = 8;
		public readonly int TotalHardnessofSoftWatertoMedicalFacilityPPM = 9;
		public readonly int TotalHardnessofSoftWatertoDentalFacilityPPM = 10;
		public readonly int TemperatureoftheCondenserMake_upWaterSupply = 11;
		public readonly int TotalSilicaSiO2Concentration = 12;
		public readonly int TotalInsolubleConcentration = 13;
		public readonly int TotalIronFeConcentration = 14;
		public readonly int TotalAluminumConcentration = 15;
		public readonly int LangelierIndexCondenserSmall = 16;
		public readonly int TotalDissolvedSolidsCondenserSmall = 17;
		public readonly int TotalCalciumHardnessCondenserSmall = 18;
		public readonly int TotalSilicaSiO2ConcentrationCondenserSmall = 19;
		public readonly int TotalpHCondenserSmall = 20;
		public readonly int TotalAlkalinityCondenserSmall = 21;
		public readonly int ConductivityCondenserSmall = 22;
		public readonly int PuckoriusIndexCondenserLarge = 23;
		public readonly int TotalPhosphateConcentrationCondenserLarge = 24;
		public readonly int TotalZincZnifusedConcentrationCondenserLarge = 25;
		public readonly int TotalMolybdateMoifusedConcentrationCondenserLarge = 26;
		public readonly int TotalPolymerConcentrationCondenserLarge = 27;
		public readonly int TotalTTConcentrationCondenserLarge = 28;
		public readonly int TotalBiocidesConcentrationCondenserLarge = 29;
		public readonly int TotalpHBoraxNitriteTreatmentChilledWater = 30;
		public readonly int TotalpHMolybdateTreatmentChilledWater = 31;
		public readonly int TotalNitriteforBorax_NitriteSystemChilledWater = 32;
		public readonly int TotalMolybdateforMolybdateTreatmentChilledWater = 33;
		public readonly int TotalTTorMBTCopperCorrosionInhibitorConcentrationChilledWater = 34;
		public readonly int ConductivityChilledWater = 35;
		public readonly int TotalGlycolConcentrationmilsperyearChilledWater = 36;
		public readonly int TotalpHBoraxNitriteTreatmentLow_MedTempHotWater = 37;
		public readonly int TotalpHforMolybdateTreatmentLow_MedTempHotWater = 38;
		public readonly int TotalNitriteforBorax_NitriteSystemLow_MedTempHotWater = 39;
		public readonly int TotalMolybdateforMolybdateTreatmentLow_MedTempHotWater = 40;
		public readonly int TotalTTorMBTCopperInhibitorConcentrationLow_MedTempHotWater = 41;
		public readonly int TotalpHHighTempHotWater = 42;
		public readonly int TotalSulfiteHighTempHotWater = 43;
		public readonly int TotalHardnessHighTempHotWater = 44;
		public readonly int TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterSmallSystems = 45;
		public readonly int TotalPhosphatePO4ConcentrationoftheBoilerWaterSmallSystems = 46;
		public readonly int TotalPolymerConcentrationoftheBoilerWaterSmallSystems = 47;
		public readonly int TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterSmallSystems = 48;
		public readonly int TotalIronFeConcentrationoftheBoilerWaterSmallSystems = 49;
		public readonly int TotalpHCondensateSmallSystems = 50;
		public readonly int ConductivityofCondensateSmallSystems = 51;
		public readonly int TotalHardnessofCondensateSmallSystems = 52;
		public readonly int TotalHardnessofMake_upWaterSmallSystems = 53;
		public readonly int TotalSulfiteNaSO3ConcentrationoftheBoilerWaterSmallSystems = 54;
		public readonly int TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterMediumSystems = 55;
		public readonly int TotalPhosphatePO4ConcentrationoftheBoilerWaterMediumSystems = 56;
		public readonly int TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterMediumSystems = 57;
		public readonly int TotalDissolvedSolidsFireTubeBoilersConcentrationoftheBoilerWaterMediumSystems = 58;
		public readonly int TotalDissolvedOxygenConcentrationoftheBoilerWaterMediumSystems = 59;
		public readonly int TotalIronFeConcentrationoftheBoilerWaterMediumSystems = 60;
		public readonly int TotalpHCondensateMediumSystems = 61;
		public readonly int ConductivityofCondensateMediumSystems = 62;
		public readonly int TotalHardnessofCondensateMediumSystems = 63;
		public readonly int TotalHardnessofMake_upWaterMediumSystems = 64;
		public readonly int TotalSulfiteNaSO3ConcentrationoftheBoilerWaterMediumSystems = 65;
		public readonly int TotalCausticityOHofMake_upWaterLargeSystems = 66;
		public readonly int TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterLargeSystems = 67;
		public readonly int TotalPhosphatePO4ConcentrationoftheBoilerWaterLargeSystems = 68;
		public readonly int TotalPolymerConcentrationoftheBoilerWaterLargeSystems = 69;
		public readonly int TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterLargeSystems = 70;
		public readonly int TotalDissolvedSolidsFireTubeBoilersConcentrationoftheBoilerWaterLargeSystems = 71;
		public readonly int TotalSuspendedSolidsConcentrationoftheBoilerWaterLargeSystems = 72;
		public readonly int TotalSodiumSulfateConcentrationoftheBoilerWaterLargeSystems = 73;
		public readonly int TotalSilicaConcentrationoftheBoilerWaterLargeSystems = 74;
		public readonly int TotalDissolvedOxygenConcentrationoftheBoilerWaterLargeSystems = 75;
		public readonly int TotalIronFeConcentrationoftheBoilerWaterLargeSystems = 76;
		public readonly int TotalpHCondensateLargeSystems = 77;
		public readonly int ConductivityoftheMake_upWaterLargeSystems = 78;
		public readonly int ConductivityofCondensateLargeSystems = 79;
		public readonly int NeutralizedConductivityLargeSystems = 80;
		public readonly int TotalHardnessofCondensateLargeSystems = 81;
		public readonly int TotalHardnessofMake_upWaterLargeSystems = 82;
		public readonly int TotalSulfiteNaSO3ConcentrationoftheBoilerWaterLargeSystem = 83;
		public readonly int LangelierIndexCondenserLarge = 84;
		public readonly int TotalDissolvedSolidsPPMoftheCondenserWaterLargeSystem = 85;
		public readonly int TotalCalciumHardnessPPMoftheCondenserWaterLargeSystem = 86;
		public readonly int TotalSilicaSiO2ConcentrationPPMoftheCondenserWaterLargeSystem = 87;
		public readonly int TotalpHLargeSystem = 88;
		public readonly int TotalAlkalinityoftheCondenserWaterLargeSystem = 89;
		public readonly int ConductivityoftheCondenserWaterLargeSystem = 90;
		public readonly int TotalpHCoolingTowerQA = 91;
		public readonly int TotalSilicaConcentrationCoolingTowerQA = 92;
		public readonly int TotalIronFeConcentrationCoolingTowerQA = 93;
		public readonly int TotalCalciumHardnessCoolingTowerQA = 94;
		public readonly int TotalHardnessCoolingTowerQA = 95;
		public readonly int TotalChlorideConcentrationCoolingTowerQA = 96;
		public readonly int TotalAlkalinityCoolingTowerQA = 97;
		public readonly int ConductivityCoolingTowerQA = 98;
		public readonly int PresenceofScale_CorrosionCondenserSmallQA = 99;
		public readonly int TotalPhosphateConcentrationCondenserSmallQA = 100;
		public readonly int TotalBiocidesConcentrationCondenserSmallQA = 101;
		public readonly int TotalpHCondenserSmallQA = 102;
		public readonly int TotalCalciumHardnessCondenserSmallQA = 103;
		public readonly int TotalAlkalinityCondenserSmallQA = 104;
		public readonly int ConductivityCondenserSmallQA = 105;
		public readonly int TemperatureCondenserLargeQA = 106;
		public readonly int TotalpHCondenserLargeQA = 107;
		public readonly int TotalSilicaConcentrationCondenserLargeQA = 108;
		public readonly int TotalIronasFe2O3ConcentrationCondenserLargeQA = 109;
		public readonly int TotalCopperCuConcentrationCondenserLargeQA = 110;
		public readonly int CalciumHardnessasCaCO3ConcentrationCondenserLargeQA = 111;
		public readonly int TotalHardnessasCaCO3ConcentrationCondenserLargeQA = 112;
		public readonly int TotalChlorideCLConcentrationCondenserLargeQA = 113;
		public readonly int TotalAlkalinityCondenserLargeQA = 114;
		public readonly int ConductivityCondenserLargeQA = 115;
		public readonly int TotalDissolvedSolidsCondenserLargeQA = 116;
		public readonly int TotalPhosphateConcentrationCondenserLargeQA = 117;
		public readonly int TotalZincZnConcentrationCondenserLargeQA = 118;
		public readonly int TotalMolybdateMoConcentrationCondenserLargeQA = 119;
		public readonly int TotalTTConcentrationCondenserLargeQA = 120;
		public readonly int TotalBiocidesConcentrationCondenserLargeQA = 121;
		public readonly int BacteriaColonyCountCondenserLargeQA = 122;
		public readonly int TotalpHforBorax_NitriteTreatmentChilledWaterQA = 123;
		public readonly int TotalpHforMolybdateTreatmentChilledWaterQA = 124;
		public readonly int TotalNitriteConcentrationChilledWaterQA = 125;
		public readonly int TotalMolybdateConcentrationChilledWaterQA = 126;
		public readonly int TotalIronasFe2O3ConcentrationChilledWaterQA = 127;
		public readonly int TotalConductivityChilledWaterQA = 128;
		public readonly int TotalpHBoraxNitriteTreatmentLow_MediumTemperatureHotWaterQA = 129;
		public readonly int TotalpHMolybdateTreatmentLow_MediumTemperatureHotWaterQA = 130;
		public readonly int TotalNitriteforBorax_NitriteSystemLow_MediumTemperatureHotWaterQA = 131;
		public readonly int TotalMolybdateConcentrationLow_MediumTemperatureHotWaterQA = 132;
		public readonly int TotalIronasFe2O3ConcentrationLow_MediumTemperatureHotWaterQA = 133;
		public readonly int TotalpHHighTemperatureHotWaterQA = 134;
		public readonly int TotalSulfiteHighTemperatureHotWaterQA = 135;
		public readonly int TotalHardnessHighTemperatureHotWaterQA = 136;
		public readonly int TotalIronasFe2O3ConcentrationHighTemperatureHotWaterQA = 137;
		public readonly int TotalpHBoilerSmallQA = 138;
		public readonly int TotalSulfiteNaSO3ConcentrationBoilerSmallQA = 139;
		public readonly int TotalAlkalinityCaCO3ConcentrationBoilerSmallQA = 140;
		public readonly int TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerSmallQA = 141;
		public readonly int TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerSmallQA = 142;
		public readonly int TotalPhosphatePO4ConcentrationBoilerSmallQA = 143;
		public readonly int TotalPolymerorTanninConcentrationBoilerSmallQA = 144;
		public readonly int TotalIronFeConcentrationBoilerSmallQA = 145;
		public readonly int TotalpHCondensateBoilerSmallQA = 146;
		public readonly int ConductivityCondensateBoilerSmallQA = 147;
		public readonly int TotalHardnessCaCO3ofCondensateBoilerSmallQA = 148;
		public readonly int TotalIronFeConcentrationCondensateBoilerSmallQA = 149;
		public readonly int TotalHardnessCaCO3ofMake_upWaterBoilerSmallQA = 150;
		public readonly int TotalpHBoilerMediumQA = 151;
		public readonly int TotalSulfiteNaSO3ConcentrationBoilerMediumQA = 152;
		public readonly int TotalAlkalinityCaCO3ConcentrationBoilerMediumQA = 153;
		public readonly int TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerMediumQA = 154;
		public readonly int TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerMediumQA = 155;
		public readonly int TotalPhosphatePO4ConcentrationBoilerMediumQA = 156;
		public readonly int TotalPolymerorTanninConcentrationBoilerMediumQA = 157;
		public readonly int TotalIronFeConcentrationBoilerMediumQA = 158;
		public readonly int TotalpHCondensateBoilerMediumQA = 159;
		public readonly int ConductivityofCondensateBoilerMediumQA = 160;
		public readonly int TotalHardnessCaCO3ofCondensateBoilerMediumQA = 161;
		public readonly int TotalIronFeConcentrationCondensateBoilerMediumQA = 162;
		public readonly int TotalHardnessCaCO3ofMake_upWaterBoilerMediumQA = 163;
		public readonly int TotalpHBoilerLargeQA = 164;
		public readonly int TotalSulfiteNaSO3ConcentrationBoilerLargeQA = 165;
		public readonly int TotalAlkalinityCaCO3ConcentrationBoilerLargeQA = 166;
		public readonly int TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerLargeQA = 167;
		public readonly int TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerLargeQA = 168;
		public readonly int ConductivityBoilerLargeQA = 169;
		public readonly int NeutralizedConductivityBoilerLargeQA = 170;
		public readonly int TotalPhosphatePO4ConcentrationBoilerLargeQA = 171;
		public readonly int TotalPolymerorTanninConcentrationBoilerLargeQA = 172;
		public readonly int TotalSilicaSiO2ConcentrationBoilerLargeQA = 173;
		public readonly int TotalIronFeConcentrationBoilerLargeQA = 174;
		public readonly int TotalpHCondensateBoilerLargeQA = 175;
		public readonly int ConductivityCondensateBoilerLargeQA = 176;
		public readonly int TotalHardnessCaCO3ofCondensateBoilerLargeQA = 177;
		public readonly int TotalIronFeConcentrationCondensateBoilerLargeQA = 178;
		public readonly int TotalHardnessCaCO3ofMake_upWaterBoilerLargeQA = 179;
	}

	public class ParameterRef
	{

		protected readonly WTContext Context;

		public ParameterIds Id { get; private set; }

        public ParameterRef(WTContext context)
        {            
			Context = context;
			Id = new ParameterIds();
        }
		
		public Parameter TotalCalciumConcentrationinSupplyWatertoFacility { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalCalciumConcentrationinSupplyWatertoFacility); } }
		public Parameter TotalMagnesiumConcentrationinSupplyWatertoFacility { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalMagnesiumConcentrationinSupplyWatertoFacility); } }
		public Parameter TotalHardnessofSupplyWatertoMedicalFacility { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofSupplyWatertoMedicalFacility); } }
		public Parameter TotalHardnessofSupplyWatertoMedicalFacilityPPM { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofSupplyWatertoMedicalFacilityPPM); } }
		public Parameter TotalHardnessofSupplyWatertoDentalFacility { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofSupplyWatertoDentalFacility); } }
		public Parameter TotalHardnessofSupplyWatertoDentalFacilityPPM { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofSupplyWatertoDentalFacilityPPM); } }
		public Parameter TotalHardnessofSoftWatertoMedicalFacilityGPG { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofSoftWatertoMedicalFacilityGPG); } }
		public Parameter TotalHardnessofSoftWatertoDentalFacilityGPG { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofSoftWatertoDentalFacilityGPG); } }
		public Parameter TotalHardnessofSoftWatertoMedicalFacilityPPM { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofSoftWatertoMedicalFacilityPPM); } }
		public Parameter TotalHardnessofSoftWatertoDentalFacilityPPM { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofSoftWatertoDentalFacilityPPM); } }
		public Parameter TemperatureoftheCondenserMake_upWaterSupply { get { return Context.Set<Parameter>().First(x => x.Id == Id.TemperatureoftheCondenserMake_upWaterSupply); } }
		public Parameter TotalSilicaSiO2Concentration { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSilicaSiO2Concentration); } }
		public Parameter TotalInsolubleConcentration { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalInsolubleConcentration); } }
		public Parameter TotalIronFeConcentration { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentration); } }
		public Parameter TotalAluminumConcentration { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAluminumConcentration); } }
		public Parameter LangelierIndexCondenserSmall { get { return Context.Set<Parameter>().First(x => x.Id == Id.LangelierIndexCondenserSmall); } }
		public Parameter TotalDissolvedSolidsCondenserSmall { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsCondenserSmall); } }
		public Parameter TotalCalciumHardnessCondenserSmall { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalCalciumHardnessCondenserSmall); } }
		public Parameter TotalSilicaSiO2ConcentrationCondenserSmall { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSilicaSiO2ConcentrationCondenserSmall); } }
		public Parameter TotalpHCondenserSmall { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondenserSmall); } }
		public Parameter TotalAlkalinityCondenserSmall { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCondenserSmall); } }
		public Parameter ConductivityCondenserSmall { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityCondenserSmall); } }
		public Parameter PuckoriusIndexCondenserLarge { get { return Context.Set<Parameter>().First(x => x.Id == Id.PuckoriusIndexCondenserLarge); } }
		public Parameter TotalPhosphateConcentrationCondenserLarge { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphateConcentrationCondenserLarge); } }
		public Parameter TotalZincZnifusedConcentrationCondenserLarge { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalZincZnifusedConcentrationCondenserLarge); } }
		public Parameter TotalMolybdateMoifusedConcentrationCondenserLarge { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalMolybdateMoifusedConcentrationCondenserLarge); } }
		public Parameter TotalPolymerConcentrationCondenserLarge { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPolymerConcentrationCondenserLarge); } }
		public Parameter TotalTTConcentrationCondenserLarge { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalTTConcentrationCondenserLarge); } }
		public Parameter TotalBiocidesConcentrationCondenserLarge { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalBiocidesConcentrationCondenserLarge); } }
		public Parameter TotalpHBoraxNitriteTreatmentChilledWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHBoraxNitriteTreatmentChilledWater); } }
		public Parameter TotalpHMolybdateTreatmentChilledWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHMolybdateTreatmentChilledWater); } }
		public Parameter TotalNitriteforBorax_NitriteSystemChilledWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalNitriteforBorax_NitriteSystemChilledWater); } }
		public Parameter TotalMolybdateforMolybdateTreatmentChilledWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalMolybdateforMolybdateTreatmentChilledWater); } }
		public Parameter TotalTTorMBTCopperCorrosionInhibitorConcentrationChilledWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalTTorMBTCopperCorrosionInhibitorConcentrationChilledWater); } }
		public Parameter ConductivityChilledWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityChilledWater); } }
		public Parameter TotalGlycolConcentrationmilsperyearChilledWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalGlycolConcentrationmilsperyearChilledWater); } }
		public Parameter TotalpHBoraxNitriteTreatmentLow_MedTempHotWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHBoraxNitriteTreatmentLow_MedTempHotWater); } }
		public Parameter TotalpHforMolybdateTreatmentLow_MedTempHotWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHforMolybdateTreatmentLow_MedTempHotWater); } }
		public Parameter TotalNitriteforBorax_NitriteSystemLow_MedTempHotWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalNitriteforBorax_NitriteSystemLow_MedTempHotWater); } }
		public Parameter TotalMolybdateforMolybdateTreatmentLow_MedTempHotWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalMolybdateforMolybdateTreatmentLow_MedTempHotWater); } }
		public Parameter TotalTTorMBTCopperInhibitorConcentrationLow_MedTempHotWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalTTorMBTCopperInhibitorConcentrationLow_MedTempHotWater); } }
		public Parameter TotalpHHighTempHotWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHHighTempHotWater); } }
		public Parameter TotalSulfiteHighTempHotWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSulfiteHighTempHotWater); } }
		public Parameter TotalHardnessHighTempHotWater { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessHighTempHotWater); } }
		public Parameter TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterSmallSystems); } }
		public Parameter TotalPhosphatePO4ConcentrationoftheBoilerWaterSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphatePO4ConcentrationoftheBoilerWaterSmallSystems); } }
		public Parameter TotalPolymerConcentrationoftheBoilerWaterSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPolymerConcentrationoftheBoilerWaterSmallSystems); } }
		public Parameter TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterSmallSystems); } }
		public Parameter TotalIronFeConcentrationoftheBoilerWaterSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationoftheBoilerWaterSmallSystems); } }
		public Parameter TotalpHCondensateSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondensateSmallSystems); } }
		public Parameter ConductivityofCondensateSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityofCondensateSmallSystems); } }
		public Parameter TotalHardnessofCondensateSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofCondensateSmallSystems); } }
		public Parameter TotalHardnessofMake_upWaterSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofMake_upWaterSmallSystems); } }
		public Parameter TotalSulfiteNaSO3ConcentrationoftheBoilerWaterSmallSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSulfiteNaSO3ConcentrationoftheBoilerWaterSmallSystems); } }
		public Parameter TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterMediumSystems); } }
		public Parameter TotalPhosphatePO4ConcentrationoftheBoilerWaterMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphatePO4ConcentrationoftheBoilerWaterMediumSystems); } }
		public Parameter TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterMediumSystems); } }
		public Parameter TotalDissolvedSolidsFireTubeBoilersConcentrationoftheBoilerWaterMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsFireTubeBoilersConcentrationoftheBoilerWaterMediumSystems); } }
		public Parameter TotalDissolvedOxygenConcentrationoftheBoilerWaterMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedOxygenConcentrationoftheBoilerWaterMediumSystems); } }
		public Parameter TotalIronFeConcentrationoftheBoilerWaterMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationoftheBoilerWaterMediumSystems); } }
		public Parameter TotalpHCondensateMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondensateMediumSystems); } }
		public Parameter ConductivityofCondensateMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityofCondensateMediumSystems); } }
		public Parameter TotalHardnessofCondensateMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofCondensateMediumSystems); } }
		public Parameter TotalHardnessofMake_upWaterMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofMake_upWaterMediumSystems); } }
		public Parameter TotalSulfiteNaSO3ConcentrationoftheBoilerWaterMediumSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSulfiteNaSO3ConcentrationoftheBoilerWaterMediumSystems); } }
		public Parameter TotalCausticityOHofMake_upWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalCausticityOHofMake_upWaterLargeSystems); } }
		public Parameter TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCaCO3ConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalPhosphatePO4ConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphatePO4ConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalPolymerConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPolymerConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsWaterTubeBoilersConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalDissolvedSolidsFireTubeBoilersConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsFireTubeBoilersConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalSuspendedSolidsConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSuspendedSolidsConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalSodiumSulfateConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSodiumSulfateConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalSilicaConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSilicaConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalDissolvedOxygenConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedOxygenConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalIronFeConcentrationoftheBoilerWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationoftheBoilerWaterLargeSystems); } }
		public Parameter TotalpHCondensateLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondensateLargeSystems); } }
		public Parameter ConductivityoftheMake_upWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityoftheMake_upWaterLargeSystems); } }
		public Parameter ConductivityofCondensateLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityofCondensateLargeSystems); } }
		public Parameter NeutralizedConductivityLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.NeutralizedConductivityLargeSystems); } }
		public Parameter TotalHardnessofCondensateLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofCondensateLargeSystems); } }
		public Parameter TotalHardnessofMake_upWaterLargeSystems { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessofMake_upWaterLargeSystems); } }
		public Parameter TotalSulfiteNaSO3ConcentrationoftheBoilerWaterLargeSystem { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSulfiteNaSO3ConcentrationoftheBoilerWaterLargeSystem); } }
		public Parameter LangelierIndexCondenserLarge { get { return Context.Set<Parameter>().First(x => x.Id == Id.LangelierIndexCondenserLarge); } }
		public Parameter TotalDissolvedSolidsPPMoftheCondenserWaterLargeSystem { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsPPMoftheCondenserWaterLargeSystem); } }
		public Parameter TotalCalciumHardnessPPMoftheCondenserWaterLargeSystem { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalCalciumHardnessPPMoftheCondenserWaterLargeSystem); } }
		public Parameter TotalSilicaSiO2ConcentrationPPMoftheCondenserWaterLargeSystem { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSilicaSiO2ConcentrationPPMoftheCondenserWaterLargeSystem); } }
		public Parameter TotalpHLargeSystem { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHLargeSystem); } }
		public Parameter TotalAlkalinityoftheCondenserWaterLargeSystem { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityoftheCondenserWaterLargeSystem); } }
		public Parameter ConductivityoftheCondenserWaterLargeSystem { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityoftheCondenserWaterLargeSystem); } }
		public Parameter TotalpHCoolingTowerQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCoolingTowerQA); } }
		public Parameter TotalSilicaConcentrationCoolingTowerQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSilicaConcentrationCoolingTowerQA); } }
		public Parameter TotalIronFeConcentrationCoolingTowerQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationCoolingTowerQA); } }
		public Parameter TotalCalciumHardnessCoolingTowerQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalCalciumHardnessCoolingTowerQA); } }
		public Parameter TotalHardnessCoolingTowerQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessCoolingTowerQA); } }
		public Parameter TotalChlorideConcentrationCoolingTowerQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalChlorideConcentrationCoolingTowerQA); } }
		public Parameter TotalAlkalinityCoolingTowerQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCoolingTowerQA); } }
		public Parameter ConductivityCoolingTowerQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityCoolingTowerQA); } }
		public Parameter PresenceofScale_CorrosionCondenserSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.PresenceofScale_CorrosionCondenserSmallQA); } }
		public Parameter TotalPhosphateConcentrationCondenserSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphateConcentrationCondenserSmallQA); } }
		public Parameter TotalBiocidesConcentrationCondenserSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalBiocidesConcentrationCondenserSmallQA); } }
		public Parameter TotalpHCondenserSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondenserSmallQA); } }
		public Parameter TotalCalciumHardnessCondenserSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalCalciumHardnessCondenserSmallQA); } }
		public Parameter TotalAlkalinityCondenserSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCondenserSmallQA); } }
		public Parameter ConductivityCondenserSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityCondenserSmallQA); } }
		public Parameter TemperatureCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TemperatureCondenserLargeQA); } }
		public Parameter TotalpHCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondenserLargeQA); } }
		public Parameter TotalSilicaConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSilicaConcentrationCondenserLargeQA); } }
		public Parameter TotalIronasFe2O3ConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronasFe2O3ConcentrationCondenserLargeQA); } }
		public Parameter TotalCopperCuConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalCopperCuConcentrationCondenserLargeQA); } }
		public Parameter CalciumHardnessasCaCO3ConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.CalciumHardnessasCaCO3ConcentrationCondenserLargeQA); } }
		public Parameter TotalHardnessasCaCO3ConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessasCaCO3ConcentrationCondenserLargeQA); } }
		public Parameter TotalChlorideCLConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalChlorideCLConcentrationCondenserLargeQA); } }
		public Parameter TotalAlkalinityCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCondenserLargeQA); } }
		public Parameter ConductivityCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityCondenserLargeQA); } }
		public Parameter TotalDissolvedSolidsCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsCondenserLargeQA); } }
		public Parameter TotalPhosphateConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphateConcentrationCondenserLargeQA); } }
		public Parameter TotalZincZnConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalZincZnConcentrationCondenserLargeQA); } }
		public Parameter TotalMolybdateMoConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalMolybdateMoConcentrationCondenserLargeQA); } }
		public Parameter TotalTTConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalTTConcentrationCondenserLargeQA); } }
		public Parameter TotalBiocidesConcentrationCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalBiocidesConcentrationCondenserLargeQA); } }
		public Parameter BacteriaColonyCountCondenserLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.BacteriaColonyCountCondenserLargeQA); } }
		public Parameter TotalpHforBorax_NitriteTreatmentChilledWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHforBorax_NitriteTreatmentChilledWaterQA); } }
		public Parameter TotalpHforMolybdateTreatmentChilledWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHforMolybdateTreatmentChilledWaterQA); } }
		public Parameter TotalNitriteConcentrationChilledWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalNitriteConcentrationChilledWaterQA); } }
		public Parameter TotalMolybdateConcentrationChilledWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalMolybdateConcentrationChilledWaterQA); } }
		public Parameter TotalIronasFe2O3ConcentrationChilledWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronasFe2O3ConcentrationChilledWaterQA); } }
		public Parameter TotalConductivityChilledWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalConductivityChilledWaterQA); } }
		public Parameter TotalpHBoraxNitriteTreatmentLow_MediumTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHBoraxNitriteTreatmentLow_MediumTemperatureHotWaterQA); } }
		public Parameter TotalpHMolybdateTreatmentLow_MediumTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHMolybdateTreatmentLow_MediumTemperatureHotWaterQA); } }
		public Parameter TotalNitriteforBorax_NitriteSystemLow_MediumTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalNitriteforBorax_NitriteSystemLow_MediumTemperatureHotWaterQA); } }
		public Parameter TotalMolybdateConcentrationLow_MediumTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalMolybdateConcentrationLow_MediumTemperatureHotWaterQA); } }
		public Parameter TotalIronasFe2O3ConcentrationLow_MediumTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronasFe2O3ConcentrationLow_MediumTemperatureHotWaterQA); } }
		public Parameter TotalpHHighTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHHighTemperatureHotWaterQA); } }
		public Parameter TotalSulfiteHighTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSulfiteHighTemperatureHotWaterQA); } }
		public Parameter TotalHardnessHighTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessHighTemperatureHotWaterQA); } }
		public Parameter TotalIronasFe2O3ConcentrationHighTemperatureHotWaterQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronasFe2O3ConcentrationHighTemperatureHotWaterQA); } }
		public Parameter TotalpHBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHBoilerSmallQA); } }
		public Parameter TotalSulfiteNaSO3ConcentrationBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSulfiteNaSO3ConcentrationBoilerSmallQA); } }
		public Parameter TotalAlkalinityCaCO3ConcentrationBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCaCO3ConcentrationBoilerSmallQA); } }
		public Parameter TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerSmallQA); } }
		public Parameter TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerSmallQA); } }
		public Parameter TotalPhosphatePO4ConcentrationBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphatePO4ConcentrationBoilerSmallQA); } }
		public Parameter TotalPolymerorTanninConcentrationBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPolymerorTanninConcentrationBoilerSmallQA); } }
		public Parameter TotalIronFeConcentrationBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationBoilerSmallQA); } }
		public Parameter TotalpHCondensateBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondensateBoilerSmallQA); } }
		public Parameter ConductivityCondensateBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityCondensateBoilerSmallQA); } }
		public Parameter TotalHardnessCaCO3ofCondensateBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessCaCO3ofCondensateBoilerSmallQA); } }
		public Parameter TotalIronFeConcentrationCondensateBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationCondensateBoilerSmallQA); } }
		public Parameter TotalHardnessCaCO3ofMake_upWaterBoilerSmallQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessCaCO3ofMake_upWaterBoilerSmallQA); } }
		public Parameter TotalpHBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHBoilerMediumQA); } }
		public Parameter TotalSulfiteNaSO3ConcentrationBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSulfiteNaSO3ConcentrationBoilerMediumQA); } }
		public Parameter TotalAlkalinityCaCO3ConcentrationBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCaCO3ConcentrationBoilerMediumQA); } }
		public Parameter TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerMediumQA); } }
		public Parameter TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerMediumQA); } }
		public Parameter TotalPhosphatePO4ConcentrationBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphatePO4ConcentrationBoilerMediumQA); } }
		public Parameter TotalPolymerorTanninConcentrationBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPolymerorTanninConcentrationBoilerMediumQA); } }
		public Parameter TotalIronFeConcentrationBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationBoilerMediumQA); } }
		public Parameter TotalpHCondensateBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondensateBoilerMediumQA); } }
		public Parameter ConductivityofCondensateBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityofCondensateBoilerMediumQA); } }
		public Parameter TotalHardnessCaCO3ofCondensateBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessCaCO3ofCondensateBoilerMediumQA); } }
		public Parameter TotalIronFeConcentrationCondensateBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationCondensateBoilerMediumQA); } }
		public Parameter TotalHardnessCaCO3ofMake_upWaterBoilerMediumQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessCaCO3ofMake_upWaterBoilerMediumQA); } }
		public Parameter TotalpHBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHBoilerLargeQA); } }
		public Parameter TotalSulfiteNaSO3ConcentrationBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSulfiteNaSO3ConcentrationBoilerLargeQA); } }
		public Parameter TotalAlkalinityCaCO3ConcentrationBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalAlkalinityCaCO3ConcentrationBoilerLargeQA); } }
		public Parameter TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsWaterTubeBoilerConcentrationBoilerLargeQA); } }
		public Parameter TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalDissolvedSolidsFireTubeBoilerConcentrationBoilerLargeQA); } }
		public Parameter ConductivityBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityBoilerLargeQA); } }
		public Parameter NeutralizedConductivityBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.NeutralizedConductivityBoilerLargeQA); } }
		public Parameter TotalPhosphatePO4ConcentrationBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPhosphatePO4ConcentrationBoilerLargeQA); } }
		public Parameter TotalPolymerorTanninConcentrationBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalPolymerorTanninConcentrationBoilerLargeQA); } }
		public Parameter TotalSilicaSiO2ConcentrationBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalSilicaSiO2ConcentrationBoilerLargeQA); } }
		public Parameter TotalIronFeConcentrationBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationBoilerLargeQA); } }
		public Parameter TotalpHCondensateBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalpHCondensateBoilerLargeQA); } }
		public Parameter ConductivityCondensateBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.ConductivityCondensateBoilerLargeQA); } }
		public Parameter TotalHardnessCaCO3ofCondensateBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessCaCO3ofCondensateBoilerLargeQA); } }
		public Parameter TotalIronFeConcentrationCondensateBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalIronFeConcentrationCondensateBoilerLargeQA); } }
		public Parameter TotalHardnessCaCO3ofMake_upWaterBoilerLargeQA { get { return Context.Set<Parameter>().First(x => x.Id == Id.TotalHardnessCaCO3ofMake_upWaterBoilerLargeQA); } }

	}

	#endregion

	#region SubSection Reference Data

	public class SubSectionIds
	{
		public readonly int ViewReport = 1;
		public readonly int StartNewReport = 3;
		public readonly int ContinueReport = 4;
		public readonly int SubmitReport = 5;
		public readonly int ViewSites = 6;
		public readonly int CreateSite = 7;
		public readonly int EditSite = 8;
		public readonly int ViewSystemTypes = 9;
		public readonly int CreateSystemType = 10;
		public readonly int EditSystemType = 11;
		public readonly int ViewUsers = 12;
		public readonly int CreateUser = 13;
		public readonly int ViewVendors = 14;
		public readonly int CreateVendor = 15;
		public readonly int LandingPage = 16;
		public readonly int OutofBoundsReport = 17;
		public readonly int ViewSubmittedReports = 18;
		public readonly int ReportAnalytics = 19;
		public readonly int SiteReport = 20;
		public readonly int FrequencyReport = 21;
		public readonly int CollectionDashboard = 22;
		public readonly int SystemComparisonReport = 23;
		public readonly int MeasurementComparisonReport = 24;
	}

	public class SubSectionRef
	{

		protected readonly WTContext Context;

		public SubSectionIds Id { get; private set; }

        public SubSectionRef(WTContext context)
        {            
			Context = context;
			Id = new SubSectionIds();
        }
		
		public SubSection ViewReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.ViewReport); } }
		public SubSection StartNewReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.StartNewReport); } }
		public SubSection ContinueReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.ContinueReport); } }
		public SubSection SubmitReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.SubmitReport); } }
		public SubSection ViewSites { get { return Context.Set<SubSection>().First(x => x.Id == Id.ViewSites); } }
		public SubSection CreateSite { get { return Context.Set<SubSection>().First(x => x.Id == Id.CreateSite); } }
		public SubSection EditSite { get { return Context.Set<SubSection>().First(x => x.Id == Id.EditSite); } }
		public SubSection ViewSystemTypes { get { return Context.Set<SubSection>().First(x => x.Id == Id.ViewSystemTypes); } }
		public SubSection CreateSystemType { get { return Context.Set<SubSection>().First(x => x.Id == Id.CreateSystemType); } }
		public SubSection EditSystemType { get { return Context.Set<SubSection>().First(x => x.Id == Id.EditSystemType); } }
		public SubSection ViewUsers { get { return Context.Set<SubSection>().First(x => x.Id == Id.ViewUsers); } }
		public SubSection CreateUser { get { return Context.Set<SubSection>().First(x => x.Id == Id.CreateUser); } }
		public SubSection ViewVendors { get { return Context.Set<SubSection>().First(x => x.Id == Id.ViewVendors); } }
		public SubSection CreateVendor { get { return Context.Set<SubSection>().First(x => x.Id == Id.CreateVendor); } }
		public SubSection LandingPage { get { return Context.Set<SubSection>().First(x => x.Id == Id.LandingPage); } }
		public SubSection OutofBoundsReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.OutofBoundsReport); } }
		public SubSection ViewSubmittedReports { get { return Context.Set<SubSection>().First(x => x.Id == Id.ViewSubmittedReports); } }
		public SubSection ReportAnalytics { get { return Context.Set<SubSection>().First(x => x.Id == Id.ReportAnalytics); } }
		public SubSection SiteReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.SiteReport); } }
		public SubSection FrequencyReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.FrequencyReport); } }
		public SubSection CollectionDashboard { get { return Context.Set<SubSection>().First(x => x.Id == Id.CollectionDashboard); } }
		public SubSection SystemComparisonReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.SystemComparisonReport); } }
		public SubSection MeasurementComparisonReport { get { return Context.Set<SubSection>().First(x => x.Id == Id.MeasurementComparisonReport); } }

	}

	#endregion

	#region Role Reference Data

	public class RoleIds
	{
		public readonly int SystemAdministrator = 1;
		public readonly int ExecutiveReportViewer = 2;
		public readonly int SiteAdministrator = 3;
		public readonly int DataRecorder = 4;
		public readonly int ReportViewer = 5;
		public readonly int AuditRecorder = 6;
	}

	public class RoleRef
	{

		protected readonly WTContext Context;

		public RoleIds Id { get; private set; }

        public RoleRef(WTContext context)
        {            
			Context = context;
			Id = new RoleIds();
        }
		
		public Role SystemAdministrator { get { return Context.Set<Role>().First(x => x.Id == Id.SystemAdministrator); } }
		public Role ExecutiveReportViewer { get { return Context.Set<Role>().First(x => x.Id == Id.ExecutiveReportViewer); } }
		public Role SiteAdministrator { get { return Context.Set<Role>().First(x => x.Id == Id.SiteAdministrator); } }
		public Role DataRecorder { get { return Context.Set<Role>().First(x => x.Id == Id.DataRecorder); } }
		public Role ReportViewer { get { return Context.Set<Role>().First(x => x.Id == Id.ReportViewer); } }
		public Role AuditRecorder { get { return Context.Set<Role>().First(x => x.Id == Id.AuditRecorder); } }

	}

	#endregion

	#region SystemType Reference Data

	public class SystemTypeIds
	{
		public readonly int PotableWaterMeasurement = 1;
		public readonly int CoolingTowerMake_upWaterSupplyMeasurement = 2;
		public readonly int CondenserWaterMeasurementsLessThan50Tons = 3;
		public readonly int CondenserWaterMeasurementsLargerThan50Tons = 4;
		public readonly int ChilledWaterMeasurements = 5;
		public readonly int LowandMediumTemperatureHotWaterMeasurements = 6;
		public readonly int HighTemperatureHotWaterMeasurements = 7;
		public readonly int SteamBoilerLessthan25HP_SmallSystem15psi_300psiMeasurements = 8;
		public readonly int SteamBoiler25HP_100HP_MediumSystem15psi_300psiMeasurements = 9;
		public readonly int SteamBoilerGreaterThan100HP_LargeSystem15psi_300psiMeasurements = 10;
		public readonly int CoolingTowerMake_upWaterSupplyMeasurementQA = 11;
		public readonly int CondenserLessThan50TonsQA = 12;
		public readonly int CondenserLargerThan50TonsQA = 13;
		public readonly int ChilledWaterQA = 14;
		public readonly int Low_MediumTemperatureHotWaterQA = 15;
		public readonly int HighTemperatureHotWaterQA = 16;
		public readonly int SteamBoilerLessthan25HP_SmallSystem15psi_300psiQA = 17;
		public readonly int SteamBoiler25HP_100HP_MediumSystem15psi_300psiQA = 18;
		public readonly int SteamBoilerGreaterThan100HP_LargeSystem15psi_300psiQA = 19;
	}

	public class SystemTypeRef
	{

		protected readonly WTContext Context;

		public SystemTypeIds Id { get; private set; }

        public SystemTypeRef(WTContext context)
        {            
			Context = context;
			Id = new SystemTypeIds();
        }
		
		public SystemType PotableWaterMeasurement { get { return Context.Set<SystemType>().First(x => x.Id == Id.PotableWaterMeasurement); } }
		public SystemType CoolingTowerMake_upWaterSupplyMeasurement { get { return Context.Set<SystemType>().First(x => x.Id == Id.CoolingTowerMake_upWaterSupplyMeasurement); } }
		public SystemType CondenserWaterMeasurementsLessThan50Tons { get { return Context.Set<SystemType>().First(x => x.Id == Id.CondenserWaterMeasurementsLessThan50Tons); } }
		public SystemType CondenserWaterMeasurementsLargerThan50Tons { get { return Context.Set<SystemType>().First(x => x.Id == Id.CondenserWaterMeasurementsLargerThan50Tons); } }
		public SystemType ChilledWaterMeasurements { get { return Context.Set<SystemType>().First(x => x.Id == Id.ChilledWaterMeasurements); } }
		public SystemType LowandMediumTemperatureHotWaterMeasurements { get { return Context.Set<SystemType>().First(x => x.Id == Id.LowandMediumTemperatureHotWaterMeasurements); } }
		public SystemType HighTemperatureHotWaterMeasurements { get { return Context.Set<SystemType>().First(x => x.Id == Id.HighTemperatureHotWaterMeasurements); } }
		public SystemType SteamBoilerLessthan25HP_SmallSystem15psi_300psiMeasurements { get { return Context.Set<SystemType>().First(x => x.Id == Id.SteamBoilerLessthan25HP_SmallSystem15psi_300psiMeasurements); } }
		public SystemType SteamBoiler25HP_100HP_MediumSystem15psi_300psiMeasurements { get { return Context.Set<SystemType>().First(x => x.Id == Id.SteamBoiler25HP_100HP_MediumSystem15psi_300psiMeasurements); } }
		public SystemType SteamBoilerGreaterThan100HP_LargeSystem15psi_300psiMeasurements { get { return Context.Set<SystemType>().First(x => x.Id == Id.SteamBoilerGreaterThan100HP_LargeSystem15psi_300psiMeasurements); } }
		public SystemType CoolingTowerMake_upWaterSupplyMeasurementQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.CoolingTowerMake_upWaterSupplyMeasurementQA); } }
		public SystemType CondenserLessThan50TonsQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.CondenserLessThan50TonsQA); } }
		public SystemType CondenserLargerThan50TonsQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.CondenserLargerThan50TonsQA); } }
		public SystemType ChilledWaterQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.ChilledWaterQA); } }
		public SystemType Low_MediumTemperatureHotWaterQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.Low_MediumTemperatureHotWaterQA); } }
		public SystemType HighTemperatureHotWaterQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.HighTemperatureHotWaterQA); } }
		public SystemType SteamBoilerLessthan25HP_SmallSystem15psi_300psiQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.SteamBoilerLessthan25HP_SmallSystem15psi_300psiQA); } }
		public SystemType SteamBoiler25HP_100HP_MediumSystem15psi_300psiQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.SteamBoiler25HP_100HP_MediumSystem15psi_300psiQA); } }
		public SystemType SteamBoilerGreaterThan100HP_LargeSystem15psi_300psiQA { get { return Context.Set<SystemType>().First(x => x.Id == Id.SteamBoilerGreaterThan100HP_LargeSystem15psi_300psiQA); } }

	}

	#endregion

	#region User Reference Data

	public class UserIds
	{
		public readonly int NIKA = 1;
		public readonly int NIKATest = 2;
		public readonly int DR = 3;
		public readonly int ERV = 4;
		public readonly int RV = 5;
		public readonly int SA = 6;
		public readonly int AuditEngineer = 7;
		public readonly int AuditDataEngineer = 8;
	}

	public class UserRef
	{

		protected readonly WTContext Context;

		public UserIds Id { get; private set; }

        public UserRef(WTContext context)
        {            
			Context = context;
			Id = new UserIds();
        }
		
		public User NIKA { get { return Context.Set<User>().First(x => x.Id == Id.NIKA); } }
		public User NIKATest { get { return Context.Set<User>().First(x => x.Id == Id.NIKATest); } }
		public User DR { get { return Context.Set<User>().First(x => x.Id == Id.DR); } }
		public User ERV { get { return Context.Set<User>().First(x => x.Id == Id.ERV); } }
		public User RV { get { return Context.Set<User>().First(x => x.Id == Id.RV); } }
		public User SA { get { return Context.Set<User>().First(x => x.Id == Id.SA); } }
		public User AuditEngineer { get { return Context.Set<User>().First(x => x.Id == Id.AuditEngineer); } }
		public User AuditDataEngineer { get { return Context.Set<User>().First(x => x.Id == Id.AuditDataEngineer); } }

	}

	#endregion

	#region Building Reference Data

	public class BuildingIds
	{
		public readonly int TestBuilding = 1;
	}

	public class BuildingRef
	{

		protected readonly WTContext Context;

		public BuildingIds Id { get; private set; }

        public BuildingRef(WTContext context)
        {            
			Context = context;
			Id = new BuildingIds();
        }
		
		public Building TestBuilding { get { return Context.Set<Building>().First(x => x.Id == Id.TestBuilding); } }

	}

	#endregion

	#region Site Reference Data

	public class SiteIds
	{
		public readonly int TestSite = 1;
	}

	public class SiteRef
	{

		protected readonly WTContext Context;

		public SiteIds Id { get; private set; }

        public SiteRef(WTContext context)
        {            
			Context = context;
			Id = new SiteIds();
        }
		
		public Site TestSite { get { return Context.Set<Site>().First(x => x.Id == Id.TestSite); } }

	}

    #endregion

    #region Setting Reference Data

    public class SettingIds
    {
        public readonly int SiteDigestMailId = 1;
        public readonly int DebugQAEmailId = 2;
        public readonly string SiteDigestMailText = "SiteDigestMailText";
        public readonly string DebugQAEmailText = "DebugQAEmail";
    }

    public class SettingRef
	{

		protected readonly WTContext Context;

		public SettingIds Id { get; private set; }

        public SettingRef(WTContext context)
        {            
			Context = context;
			Id = new SettingIds();
        }
		
		public Setting SiteDigestMailText { get { return Context.Set<Setting>().First(x => x.Name == Id.SiteDigestMailText); } }
		public Setting DebugQAEmail { get { return Context.Set<Setting>().First(x => x.Name == Id.DebugQAEmailText); } }

	}

	#endregion

}

namespace WaterTreatment.Web.Migrations
{

	using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
	using System.IO;
	using System.Reflection;
	using System.Web;
	using System.Web.Hosting;
	using WaterTreatment.Web.Entities.Ref;
	using YamlDotNet.Serialization;
	using YamlDotNet.Serialization.NamingConventions;

	partial class Configuration
	{

		private void RefDataSeed(WTContext context)
		{

		#if Production
			String refName = "ReferenceData.Production";
		#else
			String refName = "ReferenceData";
		#endif

			var filePath = MapPath("~/Entities/Ref/" + refName + ".yaml");

            if (!File.Exists(filePath))
                filePath = MapPath("~/bin/Entities/Ref/" + refName + ".yaml");

            var yamlData = File.ReadAllText(filePath);
			var reader = new StringReader(yamlData);

			var deserializer = new Deserializer(namingConvention: new PascalCaseNamingConvention());
			var modelData = deserializer.Deserialize<RefDataModel>(reader);

			SeedDataForTable(context, modelData.Whitelist);
			ResetIdentity<ExtensionWhitelist>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.LandingActions);
			ResetIdentity<LandingAction>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.MainSections);
			ResetIdentity<MainSection>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.ParameterTypes);
			ResetIdentity<ParameterType>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.Parameters);
			ResetIdentity<Parameter>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.SubSections);
			ResetIdentity<SubSection>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.Roles);
			ResetIdentity<Role>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.SystemTypes);
			ResetIdentity<SystemType>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.Users);
			ResetIdentity<User>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.Buildings);
			ResetIdentity<Building>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.Sites);
			ResetIdentity<Site>(context);
			context.SaveChanges();
			SeedDataForTable(context, modelData.Settings);
			ResetIdentity<Setting>(context);
			context.SaveChanges();

			context.SaveChanges();

		}

		private static string GetTableName(Type type, WTContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.First()
                .Fragments.Single()
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }

		private void SeedDataForTable<T>(WTContext context, List<T> Data) where T : class, IEntity
		{

			var tableName = GetTableName(typeof(T), context);

			var set = context.Set<T>();
            if (!set.Any()) {   // Only do this if data doesn't already exist
                //Insures that IDs go in like they are supposed to
                context.Database.ExecuteSqlCommand(
                    String.Format("DBCC CHECKIDENT([{0}], RESEED, 1)", tableName)
                );

                context.Set<T>().AddOrUpdate(Data.ToArray());
            }

		}

		public void ResetIdentity<T>(WTContext context) where T : class, IEntity
        {
            var tableName = GetTableName(typeof(T), context);
            var lastEntity = context.Set<T>().OrderByDescending(x => x.Id).FirstOrDefault();
            var lastId = lastEntity == null ? 0 : lastEntity.Id;

			//Resets Id to the end so there are no collissions
            context.Database.ExecuteSqlCommand(
                String.Format("DBCC CHECKIDENT([{0}], RESEED, {1})", tableName, lastId)
            );
        }

		private string MapPath(string seedFile)
		{
			if(HttpContext.Current!=null)
				return HostingEnvironment.MapPath(seedFile);

			var absolutePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
			var directoryName = Path.GetDirectoryName(absolutePath);
			var path = Path.Combine(directoryName, ".." + seedFile.TrimStart('~').Replace('/','\\'));

			return path;
		}

	}

}