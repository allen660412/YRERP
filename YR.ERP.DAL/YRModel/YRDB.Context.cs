﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace YR.ERP.DAL.YRModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class Enitity : DbContext
    {
        public Enitity()
            : base("name=Enitity")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ada_tb> ada_tb { get; set; }
        public virtual DbSet<adb_tb> adb_tb { get; set; }
        public virtual DbSet<adc_tb> adc_tb { get; set; }
        public virtual DbSet<add_tb> add_tb { get; set; }
        public virtual DbSet<ade_tb> ade_tb { get; set; }
        public virtual DbSet<adm_tb> adm_tb { get; set; }
        public virtual DbSet<ado_tb> ado_tb { get; set; }
        public virtual DbSet<adp_tb> adp_tb { get; set; }
        public virtual DbSet<adq_tb> adq_tb { get; set; }
        public virtual DbSet<adx_tb> adx_tb { get; set; }
        public virtual DbSet<ady_tb> ady_tb { get; set; }
        public virtual DbSet<ata_tb> ata_tb { get; set; }
        public virtual DbSet<atb_tb> atb_tb { get; set; }
        public virtual DbSet<atc_tb> atc_tb { get; set; }
        public virtual DbSet<aza_tb> aza_tb { get; set; }
        public virtual DbSet<aze_tb> aze_tb { get; set; }
        public virtual DbSet<azf_tb> azf_tb { get; set; }
        public virtual DbSet<azp_tb> azp_tb { get; set; }
        public virtual DbSet<azq_tb> azq_tb { get; set; }
        public virtual DbSet<baa_tb> baa_tb { get; set; }
        public virtual DbSet<bab_tb> bab_tb { get; set; }
        public virtual DbSet<bea_tb> bea_tb { get; set; }
        public virtual DbSet<beb_tb> beb_tb { get; set; }
        public virtual DbSet<bec_tb> bec_tb { get; set; }
        public virtual DbSet<bef_tb> bef_tb { get; set; }
        public virtual DbSet<beg_tb> beg_tb { get; set; }
        public virtual DbSet<bej_tb> bej_tb { get; set; }
        public virtual DbSet<bek_tb> bek_tb { get; set; }
        public virtual DbSet<bel_tb> bel_tb { get; set; }
        public virtual DbSet<bga_tb> bga_tb { get; set; }
        public virtual DbSet<bgb_tb> bgb_tb { get; set; }
        public virtual DbSet<bgc_tb> bgc_tb { get; set; }
        public virtual DbSet<cac_tb> cac_tb { get; set; }
        public virtual DbSet<cba_tb> cba_tb { get; set; }
        public virtual DbSet<cea_tb> cea_tb { get; set; }
        public virtual DbSet<ceb_tb> ceb_tb { get; set; }
        public virtual DbSet<cee_tb> cee_tb { get; set; }
        public virtual DbSet<cef_tb> cef_tb { get; set; }
        public virtual DbSet<cfa_tb> cfa_tb { get; set; }
        public virtual DbSet<cfb_tb> cfb_tb { get; set; }
        public virtual DbSet<gac_tb> gac_tb { get; set; }
        public virtual DbSet<gba_tb> gba_tb { get; set; }
        public virtual DbSet<gbd_tb> gbd_tb { get; set; }
        public virtual DbSet<gbh_tb> gbh_tb { get; set; }
        public virtual DbSet<gbi_tb> gbi_tb { get; set; }
        public virtual DbSet<gbj_tb> gbj_tb { get; set; }
        public virtual DbSet<gea_tb> gea_tb { get; set; }
        public virtual DbSet<geb_tb> geb_tb { get; set; }
        public virtual DbSet<gfa_tb> gfa_tb { get; set; }
        public virtual DbSet<gfb_tb> gfb_tb { get; set; }
        public virtual DbSet<gfg_tb> gfg_tb { get; set; }
        public virtual DbSet<gfh_tb> gfh_tb { get; set; }
        public virtual DbSet<ica_tb> ica_tb { get; set; }
        public virtual DbSet<icb_tb> icb_tb { get; set; }
        public virtual DbSet<icc_tb> icc_tb { get; set; }
        public virtual DbSet<icd_tb> icd_tb { get; set; }
        public virtual DbSet<icm_tb> icm_tb { get; set; }
        public virtual DbSet<icn_tb> icn_tb { get; set; }
        public virtual DbSet<icp_tb> icp_tb { get; set; }
        public virtual DbSet<icx_tb> icx_tb { get; set; }
        public virtual DbSet<ifa_tb> ifa_tb { get; set; }
        public virtual DbSet<ifb_tb> ifb_tb { get; set; }
        public virtual DbSet<iga_tb> iga_tb { get; set; }
        public virtual DbSet<igb_tb> igb_tb { get; set; }
        public virtual DbSet<ila_tb> ila_tb { get; set; }
        public virtual DbSet<ilb_tb> ilb_tb { get; set; }
        public virtual DbSet<ima_tb> ima_tb { get; set; }
        public virtual DbSet<imb_tb> imb_tb { get; set; }
        public virtual DbSet<ina_tb> ina_tb { get; set; }
        public virtual DbSet<ipa_tb> ipa_tb { get; set; }
        public virtual DbSet<ipb_tb> ipb_tb { get; set; }
        public virtual DbSet<jja_tb> jja_tb { get; set; }
        public virtual DbSet<mea_tb> mea_tb { get; set; }
        public virtual DbSet<meb_tb> meb_tb { get; set; }
        public virtual DbSet<mfa_tb> mfa_tb { get; set; }
        public virtual DbSet<mfb_tb> mfb_tb { get; set; }
        public virtual DbSet<mga_tb> mga_tb { get; set; }
        public virtual DbSet<mgb_tb> mgb_tb { get; set; }
        public virtual DbSet<mha_tb> mha_tb { get; set; }
        public virtual DbSet<mhb_tb> mhb_tb { get; set; }
        public virtual DbSet<mia_tb> mia_tb { get; set; }
        public virtual DbSet<mib_tb> mib_tb { get; set; }
        public virtual DbSet<pba_tb> pba_tb { get; set; }
        public virtual DbSet<pbb_tb> pbb_tb { get; set; }
        public virtual DbSet<pbc_tb> pbc_tb { get; set; }
        public virtual DbSet<pca_tb> pca_tb { get; set; }
        public virtual DbSet<pcb_tb> pcb_tb { get; set; }
        public virtual DbSet<pcc_tb> pcc_tb { get; set; }
        public virtual DbSet<pdd_tb> pdd_tb { get; set; }
        public virtual DbSet<pea_tb> pea_tb { get; set; }
        public virtual DbSet<peb_tb> peb_tb { get; set; }
        public virtual DbSet<pfa_tb> pfa_tb { get; set; }
        public virtual DbSet<pfb_tb> pfb_tb { get; set; }
        public virtual DbSet<pga_tb> pga_tb { get; set; }
        public virtual DbSet<pgb_tb> pgb_tb { get; set; }
        public virtual DbSet<pha_tb> pha_tb { get; set; }
        public virtual DbSet<phb_tb> phb_tb { get; set; }
        public virtual DbSet<sba_tb> sba_tb { get; set; }
        public virtual DbSet<sbb_tb> sbb_tb { get; set; }
        public virtual DbSet<sbc_tb> sbc_tb { get; set; }
        public virtual DbSet<sbg_tb> sbg_tb { get; set; }
        public virtual DbSet<sca_tb> sca_tb { get; set; }
        public virtual DbSet<scb_tb> scb_tb { get; set; }
        public virtual DbSet<sdd_tb> sdd_tb { get; set; }
        public virtual DbSet<sea_tb> sea_tb { get; set; }
        public virtual DbSet<seb_tb> seb_tb { get; set; }
        public virtual DbSet<sfa_tb> sfa_tb { get; set; }
        public virtual DbSet<sfb_tb> sfb_tb { get; set; }
        public virtual DbSet<sga_tb> sga_tb { get; set; }
        public virtual DbSet<sgb_tb> sgb_tb { get; set; }
        public virtual DbSet<sha_tb> sha_tb { get; set; }
        public virtual DbSet<shb_tb> shb_tb { get; set; }
        public virtual DbSet<tba_tb> tba_tb { get; set; }
        public virtual DbSet<tbe_tb> tbe_tb { get; set; }
        public virtual DbSet<tca_tb> tca_tb { get; set; }
        public virtual DbSet<test> test { get; set; }
        public virtual DbSet<ica_tb_0606> ica_tb_0606 { get; set; }
        public virtual DbSet<ica_tb_0607> ica_tb_0607 { get; set; }
        public virtual DbSet<icp_tb_0606> icp_tb_0606 { get; set; }
        public virtual DbSet<icp_tb_0607> icp_tb_0607 { get; set; }
        public virtual DbSet<saV_tb_desc> saV_tb_desc { get; set; }
        public virtual DbSet<vw_admi100> vw_admi100 { get; set; }
        public virtual DbSet<vw_admi100s> vw_admi100s { get; set; }
        public virtual DbSet<vw_admi110> vw_admi110 { get; set; }
        public virtual DbSet<vw_admi110s> vw_admi110s { get; set; }
        public virtual DbSet<vw_admi120> vw_admi120 { get; set; }
        public virtual DbSet<vw_admi600> vw_admi600 { get; set; }
        public virtual DbSet<vw_admi601> vw_admi601 { get; set; }
        public virtual DbSet<vw_admi601s> vw_admi601s { get; set; }
        public virtual DbSet<vw_admi602> vw_admi602 { get; set; }
        public virtual DbSet<vw_admi602s> vw_admi602s { get; set; }
        public virtual DbSet<vw_admi610> vw_admi610 { get; set; }
        public virtual DbSet<vw_admi610s> vw_admi610s { get; set; }
        public virtual DbSet<vw_admi611> vw_admi611 { get; set; }
        public virtual DbSet<vw_admi611s> vw_admi611s { get; set; }
        public virtual DbSet<vw_admi620> vw_admi620 { get; set; }
        public virtual DbSet<vw_admi620s> vw_admi620s { get; set; }
        public virtual DbSet<vw_admi630> vw_admi630 { get; set; }
        public virtual DbSet<vw_admi630s> vw_admi630s { get; set; }
        public virtual DbSet<vw_admi640> vw_admi640 { get; set; }
        public virtual DbSet<vw_admi640s> vw_admi640s { get; set; }
        public virtual DbSet<vw_admi650> vw_admi650 { get; set; }
        public virtual DbSet<vw_admi650s> vw_admi650s { get; set; }
        public virtual DbSet<vw_admq910> vw_admq910 { get; set; }
        public virtual DbSet<vw_basi001> vw_basi001 { get; set; }
        public virtual DbSet<vw_basi010> vw_basi010 { get; set; }
        public virtual DbSet<vw_basi020> vw_basi020 { get; set; }
        public virtual DbSet<vw_basi030> vw_basi030 { get; set; }
        public virtual DbSet<vw_basi040> vw_basi040 { get; set; }
        public virtual DbSet<vw_basi040a> vw_basi040a { get; set; }
        public virtual DbSet<vw_basi040b> vw_basi040b { get; set; }
        public virtual DbSet<vw_basi050> vw_basi050 { get; set; }
        public virtual DbSet<vw_basi060> vw_basi060 { get; set; }
        public virtual DbSet<vw_basi070> vw_basi070 { get; set; }
        public virtual DbSet<vw_basi080> vw_basi080 { get; set; }
        public virtual DbSet<vw_basi090> vw_basi090 { get; set; }
        public virtual DbSet<vw_carb110> vw_carb110 { get; set; }
        public virtual DbSet<vw_carb350> vw_carb350 { get; set; }
        public virtual DbSet<vw_carb351> vw_carb351 { get; set; }
        public virtual DbSet<vw_cari010> vw_cari010 { get; set; }
        public virtual DbSet<vw_cari030> vw_cari030 { get; set; }
        public virtual DbSet<vw_carp350> vw_carp350 { get; set; }
        public virtual DbSet<vw_cart100> vw_cart100 { get; set; }
        public virtual DbSet<vw_cart100s> vw_cart100s { get; set; }
        public virtual DbSet<vw_cart110> vw_cart110 { get; set; }
        public virtual DbSet<vw_cart200> vw_cart200 { get; set; }
        public virtual DbSet<vw_cart200s> vw_cart200s { get; set; }
        public virtual DbSet<vw_cspb200> vw_cspb200 { get; set; }
        public virtual DbSet<vw_cspq100> vw_cspq100 { get; set; }
        public virtual DbSet<vw_cspr110> vw_cspr110 { get; set; }
        public virtual DbSet<vw_glab311> vw_glab311 { get; set; }
        public virtual DbSet<vw_glab312> vw_glab312 { get; set; }
        public virtual DbSet<vw_glab313> vw_glab313 { get; set; }
        public virtual DbSet<vw_glab321> vw_glab321 { get; set; }
        public virtual DbSet<vw_glai010> vw_glai010 { get; set; }
        public virtual DbSet<vw_glai100> vw_glai100 { get; set; }
        public virtual DbSet<vw_glai110> vw_glai110 { get; set; }
        public virtual DbSet<vw_glai110s> vw_glai110s { get; set; }
        public virtual DbSet<vw_glaq401> vw_glaq401 { get; set; }
        public virtual DbSet<vw_glaq401s> vw_glaq401s { get; set; }
        public virtual DbSet<vw_glaq402> vw_glaq402 { get; set; }
        public virtual DbSet<vw_glaq402s> vw_glaq402s { get; set; }
        public virtual DbSet<vw_glaq403> vw_glaq403 { get; set; }
        public virtual DbSet<vw_glaq403s> vw_glaq403s { get; set; }
        public virtual DbSet<vw_glaq410> vw_glaq410 { get; set; }
        public virtual DbSet<vw_glaq410s> vw_glaq410s { get; set; }
        public virtual DbSet<vw_glar300> vw_glar300 { get; set; }
        public virtual DbSet<vw_glar301> vw_glar301 { get; set; }
        public virtual DbSet<vw_glar311> vw_glar311 { get; set; }
        public virtual DbSet<vw_glar321> vw_glar321 { get; set; }
        public virtual DbSet<vw_glat200> vw_glat200 { get; set; }
        public virtual DbSet<vw_glat200s> vw_glat200s { get; set; }
        public virtual DbSet<vw_glat300> vw_glat300 { get; set; }
        public virtual DbSet<vw_glat300_1> vw_glat300_1 { get; set; }
        public virtual DbSet<vw_glat300_1s> vw_glat300_1s { get; set; }
        public virtual DbSet<vw_glat300a> vw_glat300a { get; set; }
        public virtual DbSet<vw_glat300s> vw_glat300s { get; set; }
        public virtual DbSet<vw_invb500> vw_invb500 { get; set; }
        public virtual DbSet<vw_invb501> vw_invb501 { get; set; }
        public virtual DbSet<vw_invb501s> vw_invb501s { get; set; }
        public virtual DbSet<vw_invb502> vw_invb502 { get; set; }
        public virtual DbSet<vw_invb502s> vw_invb502s { get; set; }
        public virtual DbSet<vw_invb600> vw_invb600 { get; set; }
        public virtual DbSet<vw_invi010> vw_invi010 { get; set; }
        public virtual DbSet<vw_invi020> vw_invi020 { get; set; }
        public virtual DbSet<vw_invi030> vw_invi030 { get; set; }
        public virtual DbSet<vw_invi100> vw_invi100 { get; set; }
        public virtual DbSet<vw_invi100_1> vw_invi100_1 { get; set; }
        public virtual DbSet<vw_invi100_2> vw_invi100_2 { get; set; }
        public virtual DbSet<vw_invi101> vw_invi101 { get; set; }
        public virtual DbSet<vw_invi101s> vw_invi101s { get; set; }
        public virtual DbSet<vw_invi102> vw_invi102 { get; set; }
        public virtual DbSet<vw_invi103> vw_invi103 { get; set; }
        public virtual DbSet<vw_invi103s> vw_invi103s { get; set; }
        public virtual DbSet<vw_invi200> vw_invi200 { get; set; }
        public virtual DbSet<vw_invq210> vw_invq210 { get; set; }
        public virtual DbSet<vw_invq210s> vw_invq210s { get; set; }
        public virtual DbSet<vw_invq220> vw_invq220 { get; set; }
        public virtual DbSet<vw_invr101> vw_invr101 { get; set; }
        public virtual DbSet<vw_invr102> vw_invr102 { get; set; }
        public virtual DbSet<vw_invr215> vw_invr215 { get; set; }
        public virtual DbSet<vw_invr301> vw_invr301 { get; set; }
        public virtual DbSet<vw_invr302> vw_invr302 { get; set; }
        public virtual DbSet<vw_invr330> vw_invr330 { get; set; }
        public virtual DbSet<vw_invr336> vw_invr336 { get; set; }
        public virtual DbSet<vw_invr337> vw_invr337 { get; set; }
        public virtual DbSet<vw_invr401> vw_invr401 { get; set; }
        public virtual DbSet<vw_invr402> vw_invr402 { get; set; }
        public virtual DbSet<vw_invr410> vw_invr410 { get; set; }
        public virtual DbSet<vw_invr520> vw_invr520 { get; set; }
        public virtual DbSet<vw_invr521> vw_invr521 { get; set; }
        public virtual DbSet<vw_invr522> vw_invr522 { get; set; }
        public virtual DbSet<vw_invt301> vw_invt301 { get; set; }
        public virtual DbSet<vw_invt301s> vw_invt301s { get; set; }
        public virtual DbSet<vw_invt302> vw_invt302 { get; set; }
        public virtual DbSet<vw_invt302s> vw_invt302s { get; set; }
        public virtual DbSet<vw_invt330> vw_invt330 { get; set; }
        public virtual DbSet<vw_invt330s> vw_invt330s { get; set; }
        public virtual DbSet<vw_invt331> vw_invt331 { get; set; }
        public virtual DbSet<vw_invt331s> vw_invt331s { get; set; }
        public virtual DbSet<vw_invt332> vw_invt332 { get; set; }
        public virtual DbSet<vw_invt332s> vw_invt332s { get; set; }
        public virtual DbSet<vw_invt401> vw_invt401 { get; set; }
        public virtual DbSet<vw_invt401s> vw_invt401s { get; set; }
        public virtual DbSet<vw_invt402> vw_invt402 { get; set; }
        public virtual DbSet<vw_invt402s> vw_invt402s { get; set; }
        public virtual DbSet<vw_invt520> vw_invt520 { get; set; }
        public virtual DbSet<vw_invt520s> vw_invt520s { get; set; }
        public virtual DbSet<vw_invt521> vw_invt521 { get; set; }
        public virtual DbSet<vw_invt521s> vw_invt521s { get; set; }
        public virtual DbSet<vw_invt522> vw_invt522 { get; set; }
        public virtual DbSet<vw_invt522s> vw_invt522s { get; set; }
        public virtual DbSet<vw_manr210> vw_manr210 { get; set; }
        public virtual DbSet<vw_manr311> vw_manr311 { get; set; }
        public virtual DbSet<vw_manr312> vw_manr312 { get; set; }
        public virtual DbSet<vw_manr411> vw_manr411 { get; set; }
        public virtual DbSet<vw_mant210> vw_mant210 { get; set; }
        public virtual DbSet<vw_mant210_1> vw_mant210_1 { get; set; }
        public virtual DbSet<vw_mant210s> vw_mant210s { get; set; }
        public virtual DbSet<vw_mant311> vw_mant311 { get; set; }
        public virtual DbSet<vw_mant311s> vw_mant311s { get; set; }
        public virtual DbSet<vw_mant312> vw_mant312 { get; set; }
        public virtual DbSet<vw_mant312s> vw_mant312s { get; set; }
        public virtual DbSet<vw_mant411> vw_mant411 { get; set; }
        public virtual DbSet<vw_mant411s> vw_mant411s { get; set; }
        public virtual DbSet<vw_mant412> vw_mant412 { get; set; }
        public virtual DbSet<vw_mant412s> vw_mant412s { get; set; }
        public virtual DbSet<vw_pick_ica1> vw_pick_ica1 { get; set; }
        public virtual DbSet<vw_puri020> vw_puri020 { get; set; }
        public virtual DbSet<vw_puri030> vw_puri030 { get; set; }
        public virtual DbSet<vw_puri030s> vw_puri030s { get; set; }
        public virtual DbSet<vw_puri031> vw_puri031 { get; set; }
        public virtual DbSet<vw_puri031s> vw_puri031s { get; set; }
        public virtual DbSet<vw_puri100> vw_puri100 { get; set; }
        public virtual DbSet<vw_puri100s> vw_puri100s { get; set; }
        public virtual DbSet<vw_puri101> vw_puri101 { get; set; }
        public virtual DbSet<vw_purr200> vw_purr200 { get; set; }
        public virtual DbSet<vw_purr300> vw_purr300 { get; set; }
        public virtual DbSet<vw_purr400> vw_purr400 { get; set; }
        public virtual DbSet<vw_purr410> vw_purr410 { get; set; }
        public virtual DbSet<vw_purr500> vw_purr500 { get; set; }
        public virtual DbSet<vw_purt200> vw_purt200 { get; set; }
        public virtual DbSet<vw_purt200s> vw_purt200s { get; set; }
        public virtual DbSet<vw_purt300> vw_purt300 { get; set; }
        public virtual DbSet<vw_purt300s> vw_purt300s { get; set; }
        public virtual DbSet<vw_purt400> vw_purt400 { get; set; }
        public virtual DbSet<vw_purt400s> vw_purt400s { get; set; }
        public virtual DbSet<vw_purt500> vw_purt500 { get; set; }
        public virtual DbSet<vw_purt500s> vw_purt500s { get; set; }
        public virtual DbSet<vw_stpb400> vw_stpb400 { get; set; }
        public virtual DbSet<vw_stpi010> vw_stpi010 { get; set; }
        public virtual DbSet<vw_stpi020> vw_stpi020 { get; set; }
        public virtual DbSet<vw_stpi030> vw_stpi030 { get; set; }
        public virtual DbSet<vw_stpi030s> vw_stpi030s { get; set; }
        public virtual DbSet<vw_stpi031> vw_stpi031 { get; set; }
        public virtual DbSet<vw_stpi031s> vw_stpi031s { get; set; }
        public virtual DbSet<vw_stpi040> vw_stpi040 { get; set; }
        public virtual DbSet<vw_stpi100> vw_stpi100 { get; set; }
        public virtual DbSet<vw_stpi100s> vw_stpi100s { get; set; }
        public virtual DbSet<vw_stpr200> vw_stpr200 { get; set; }
        public virtual DbSet<vw_stpr300> vw_stpr300 { get; set; }
        public virtual DbSet<vw_stpr400> vw_stpr400 { get; set; }
        public virtual DbSet<vw_stpr410> vw_stpr410 { get; set; }
        public virtual DbSet<vw_stpr500> vw_stpr500 { get; set; }
        public virtual DbSet<vw_stpr501> vw_stpr501 { get; set; }
        public virtual DbSet<vw_stpt200> vw_stpt200 { get; set; }
        public virtual DbSet<vw_stpt200s> vw_stpt200s { get; set; }
        public virtual DbSet<vw_stpt300> vw_stpt300 { get; set; }
        public virtual DbSet<vw_stpt300s> vw_stpt300s { get; set; }
        public virtual DbSet<vw_stpt400> vw_stpt400 { get; set; }
        public virtual DbSet<vw_stpt400s> vw_stpt400s { get; set; }
        public virtual DbSet<vw_stpt410> vw_stpt410 { get; set; }
        public virtual DbSet<vw_stpt410s> vw_stpt410s { get; set; }
        public virtual DbSet<vw_stpt500> vw_stpt500 { get; set; }
        public virtual DbSet<vw_stpt500s> vw_stpt500s { get; set; }
        public virtual DbSet<vw_taxb010> vw_taxb010 { get; set; }
        public virtual DbSet<vw_taxi001> vw_taxi001 { get; set; }
        public virtual DbSet<vw_taxi020> vw_taxi020 { get; set; }
        public virtual DbSet<vw_taxi020s> vw_taxi020s { get; set; }
        public virtual DbSet<vw_test> vw_test { get; set; }
        public virtual DbSet<vw_xinvt302> vw_xinvt302 { get; set; }
        public virtual DbSet<vw_xinvt302s> vw_xinvt302s { get; set; }
        public virtual DbSet<vw_zinvt001> vw_zinvt001 { get; set; }
        public virtual DbSet<vw_zinvt001s> vw_zinvt001s { get; set; }
    
        public virtual int sap_check_table(string table)
        {
            var tableParameter = table != null ?
                new ObjectParameter("table", table) :
                new ObjectParameter("table", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sap_check_table", tableParameter);
        }
    
        public virtual int sap_update_aza04(string table_name, string view_name)
        {
            var table_nameParameter = table_name != null ?
                new ObjectParameter("table_name", table_name) :
                new ObjectParameter("table_name", typeof(string));
    
            var view_nameParameter = view_name != null ?
                new ObjectParameter("view_name", view_name) :
                new ObjectParameter("view_name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sap_update_aza04", table_nameParameter, view_nameParameter);
        }
    
        public virtual int sp_gen_aza(string p_view)
        {
            var p_viewParameter = p_view != null ?
                new ObjectParameter("p_view", p_view) :
                new ObjectParameter("p_view", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_gen_aza", p_viewParameter);
        }
    
        public virtual ObjectResult<sp_invr450_Result> sp_invr450(string p_view)
        {
            var p_viewParameter = p_view != null ?
                new ObjectParameter("p_view", p_view) :
                new ObjectParameter("p_view", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_invr450_Result>("sp_invr450", p_viewParameter);
        }
    }
}
