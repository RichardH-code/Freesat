<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes" version="1.0" encoding="utf-8" omit-xml-declaration="no" standalone="no"/>
	<xsl:strip-space elements="*"/>
	<xsl:param name="satellite_name"/>
	<xsl:param name="satellite_positionEast"/>
	<xsl:variable name="satellite_uid" select="concat('!DvbsDataSet!DvbsSatellite[',$satellite_positionEast,']')"/>
	<xsl:param name="region_isoCode"/>
	<xsl:variable name="region_uid" select="concat('!DvbsDataSet!DvbsRegion[',$region_isoCode,']')"/>
	<xsl:variable name="footprint_uid" select="concat('!DvbsDataSet!DvbsFootprint[',$region_isoCode,':',$satellite_positionEast,']')"/>
	<xsl:param name="headend_csiId"/>
	<xsl:param name="headend_languageIso639"/>
	<xsl:variable name="headend_uid" select="concat('!DvbsDataSet!DvbsHeadend[',$headend_csiId,']')"/>
	<xsl:template match="/">
		<MXF>
			<Assembly name="mcepg" version="6.0.6000.0" cultureInfo="" publicKey="0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9">
				<NameSpace name="Microsoft.MediaCenter.Satellites">
					<Type name="DvbsDataSet"/>
					<Type name="DvbsSatellite"/>
					<Type name="DvbsRegion"/>
					<Type name="DvbsHeadend"/>
					<Type name="DvbsTransponder"/>
					<Type name="DvbsFootprint"/>
					<Type name="DvbsChannel"/>
					<Type name="DvbsService"/>
				</NameSpace>
			</Assembly>
			<Assembly name="mcstore" version="6.0.6000.0" cultureInfo="" publicKey="0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9">
				<NameSpace name="Microsoft.MediaCenter.Store">
					<Type name="Provider"/>
					<Type name="UId" parentFieldName="target"/>
				</NameSpace>
			</Assembly>
			<DvbsDataSet uid="!DvbsDataSet" _frequencyTolerance="10" _symbolRateTolerance="1500" _minimumSearchMatches="3" _dataSetRevision="90">
				<_allSatellites>
					<DvbsSatellite uid="{$satellite_uid}" _name="{$satellite_name}" _positionEast="{$satellite_positionEast}">
						<_transponders>
							<xsl:for-each select="//DvbsTransponder[starts-with(@uid,$satellite_uid)]">
								<xsl:variable name="transponder_uid" select="@uid"/>
								<xsl:copy>
									<xsl:apply-templates select="@*|node()"/>
									<xsl:element name="_services">
										<xsl:copy-of select="//DvbsService[starts-with(@uid,$transponder_uid)]"/>
									</xsl:element>
								</xsl:copy>
							</xsl:for-each>
						</_transponders>
					</DvbsSatellite>
				</_allSatellites>
				<_allRegions>
					<DvbsRegion uid="{$region_uid}" _isoCode="{$region_isoCode}">
						<_footprints>
							<DvbsFootprint uid="{$footprint_uid}" _satellite="{$satellite_uid}">
								<headends>
									<DvbsHeadend uid="{$headend_uid}" _csiId="{$headend_csiId}" _languageIso639="{$headend_languageIso639}">
										<_channels>
											<xsl:copy-of select="//DvbsChannel[starts-with(@uid,$headend_uid)]"/>
										</_channels>
									</DvbsHeadend>
								</headends>
							</DvbsFootprint>
						</_footprints>
					</DvbsRegion>
				</_allRegions>
				<_allHeadends>
					<DvbsHeadend idref="{$headend_uid}"/>
				</_allHeadends>
			</DvbsDataSet>
		</MXF>
	</xsl:template>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
