<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowHotelRoomPic.aspx.cs" Inherits="Ctrip.Hotel.Online.IntlHotelSearchOnlineSite.ShowHotelRoomPic" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta name="keywords" content="<%=metaKeywords %>" />
<meta name="description" content="<%=meatDescription %>" />
<meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
<title><%=pageTitle %></title>
 
<link href="http://webresource.c-ctrip.com/styles/common/cui101103.css?110228.css" rel="stylesheet" type="text/css" /><link href="http://webresource.c-ctrip.com/styles/common/public_flights_logo.css?100705.css" rel="stylesheet" type="text/css" />
<link href="http://webresource.c-ctrip.com/styles/hotelinternational/pub_hotels.css?110304.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" charset="utf-8" src="http://webresource.c-ctrip.com/code/js/tuna_090501.js"></script>
<link href="http://webresource.c-ctrip.com/styles/hotelinternational/picture.css?101110.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="http://webresource.c-ctrip.com/code/js/public/tuna_090501/intl_hotels.js"></script>
<script type="text/javascript" src="http://webresource.c-ctrip.com/code/js/private/tuna_090501/cn/hotels/intl_picture.js"></script>
</head>
<body>
<ul id="base_wrapper">
  <li id="base_hd">
    <ul class="base_pagehead">
      <li class="picture_logo">
        <h1><a href="http://www.ctrip.com/" title="携程旅行网"><img src="http://pic.c-ctrip.com/common/logo2.gif" alt="携程旅行网" width="215" height="52" /></a></h1>
      </li>
    </ul>
  </li>
  <!-- head end and body content begin -->
  <li id="base_bd" class="picture_bd">
    <div class="picture_wrap">
		<div class="picture_path">
        <asp:HyperLink ID="lnkHotelBook" runat="server" Text="酒店预订" NavigateUrl="http://pages.ctrip.com/public/sitemap/cityHotel.htm" /> &raquo; 
        <asp:HyperLink ID="lnkCityName" runat="server" /> &raquo; 
        <asp:HyperLink ID="lnkHotelName" runat="server" />
		<!-- gta -->


		<div id="hotelPic" class="picture_main layoutfix">
			<table border="0" cellpadding="0" cellspacing="0" align="center" width="740">
				<tr>
					<td height=12></td>
				</tr>
				<tr>
					<td >
						<table width="100%" border="0" cellspacing="0" cellpadding="0">
							<%=roomPicInfo%>
						</table>
					</td>
				</tr>
				<tr>
					<td  height=26> 　</td>
				</tr>
				<tr>
					<td align="center" class="picture_btns"><a href="javascript:self.close();" class="prev">关闭</a></td>
				</tr>
			</table>
		</div>
	</div>
	</li>
<!-- body content end and foot begin -->
<!-- InstanceEnd name="position" -->
<li id="base_ft" cdm="blk_pagebottom">
	<p>
		<a href="http://www.ctrip.com/Member/SignUp.asp" title="免费注册">免费注册</a> |
		<a href="http://pages.ctrip.com/public/sitemap/sitemap.htm" title="网站导航">网站导航</a> |
		<a href="http://pages.ctrip.com/public/sitemap/cityHotel.htm" title="宾馆索引">宾馆索引</a> |
		<a href="http://pages.ctrip.com/public/serve%20guideline.htm" title="服务说明">服务说明</a> |
		<a href="http://pages.ctrip.com/public/ctripab/abctrip.htm" title="关于携程">关于携程</a> |
		<a title="企业公民" href="http://pages.ctrip.com/commerce/promote_t/200908/qyxx/index.htm">企业公民</a> | 
		<a href="http://www.ctrip.com/public/job/job.asp" title="诚聘英才">诚聘英才</a> |
		<a href="http://pages.ctrip.com/public/dlhz.htm" title="代理合作">代理合作</a> |
		<a href="http://pages.ctrip.com/public/ctripad/adyw.htm" title="广告业务">广告业务</a> |
		<a href="http://www.ctrip.com/community/ue/index.asp" title="用户体验平台">用户体验平台</a> |
		<a href="http://pages.ctrip.com/public/contact.htm" title="联系我们">联系我们</a>
	</p>
	<p style="font-family: Verdana,simsun,sans-serif;">
		<a href="http://pages.ctrip.com/public/copyright.htm">Copyright&copy;</a> 1999-<%=DateTime.Today.Year.ToString()%>, <a href="http://pages.ctrip.com/public/ctripab/abctrip.htm">ctrip.com</a>. all rights reserved.</p>
</li></ul>
	<input type="hidden" id="hotel" value="<%=hotel %>" />
	<input type="hidden" id="hotel_pic" value="<%=hotel %>" />
	<input type="hidden" id="room_pic" value="<%=room%>" />

	<input id="startdate" type="hidden"  style="width:100;" name="startdate" />
	<input id="depdate" type ="hidden" style="width:100;" name="depdate" /> 
	

</body>
</html>
