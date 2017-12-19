#% text_encoding = iso8859_1
##-----------------------------------------------------------------------------------------------
## Web Maps Connector (which shows web maps as layers in Smallworld(TM) Core Spatial Technology)
## Copyright (C) 2017 KUBRA
##
## This program is free software: you can redistribute it and/or modify
## it under the terms of the GNU General Public License as published by
## the Free Software Foundation, either version 3 of the License, or
## (at your option) any later version.
##
## This program is distributed in the hope that it will be useful,
## but WITHOUT ANY WARRANTY; without even the implied warranty of
## MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
## GNU General Public License for more details.
##
## You should have received a copy of the GNU General Public License
## along with this program.  If not, see <http:##www.gnu.org/licenses/>.
##
## You may contact KUBRA at https://kubra.com/contact-us/
##-----------------------------------------------------------------------------------------------


:ve_reverse_geocode_server_error	Could not get Reverse Geocode information from the server.

:ve_metadata	 Image information for

Lon: #1  Lat: #2
Level of Detail: #3
Map type: #4

Date Range: #5 - #6


:ve_metadata_server_error	Could not get image information from the server for

Lon: #1  Lat: #2
Level of Detail: #3
Map type: #4


:ve_no_mercator_coordinate_system_defined	No 'Virtual Earth' projection has been defined in your GIS partition.

Please have your administrator run the following piece of Magik code...

_block
	 _dynamic !current_dsview! << gis_program_manager.cached_dataset(:gis)

	 !current_dsview!.goto_top_alternative()
	 !current_dsview!.switch(:write)

	 ds_coordinate_system.new_from(ve_dataset.MERCATOR_COORDINATE_SYSTEM)

	 !current_dsview!.commit()
	 !current_dsview!.switch(:readonly)

_endblock 
$


:ve_install_net_framework	You do not have a recent enough version of .NET Framework installed.  You need at least version #1.

You can download .NET Framework from #2.

:ve_update_connection_error	Cannot connect to the iFactor server to determine if an update is available.
Please verify that your connection to the internet is currently enabled.

:ve_credentials_need_to_be_loaded  Please run the credential Magik file one time after you have installed the VE Connector code.
If you do not have a suitable credential file, please contact sales@ifactorconsulting.com

:ve_transaction_mechanism_tampered_with	 Cannot launch the VE ACP.  The billing transaction mechanism has been tampered with.  Please reload the code as it was delivered to you in an official release.

:ve_invalid_sw_auth_type		 '#1' is an invalid Smallworld Authorisation Type. Please choose one of #2.

:ve_current_user_not_sw_auth_validated	 The current user does not have sufficient rights to use the Bing Maps Connector functionality.

:ve_invalid_query			 Query String is Empty
:wmc_no_search_results			 We didn't find results for your search. Try other search terms.
