<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        	$(document).ready(function(){
        		$(".mainblock a").lightBox();
                var elementheight;
                var elementwidth;
                var elementmarginheight;
                var elementmarginwidth;

                var elementimageheight;
                var elementimagewidth;

                var elementimagetdheight;
                var elementimagetdwidth;

	        	$(".element").hover (
	        		function() {

                        elementheight = $(this).height();
                        elementwidth = $(this).width();
                        //console.log("elementheight - " + elementheight);
                        elementmarginheight = ($(this).outerHeight(true) - $(this).outerHeight())/2;
                        elementmarginwidth = ($(this).outerWidth(true) - $(this).outerWidth())/2;
                        
                        elementimageheight = $(this).find('.elementimage').height();
                        elementimagewidth = $(this).find('.elementimage').width();

                        elementimagetdheight = $(this).find('.image').height();
                        elementimagetdwidth = $(this).find('.image').width();

                        var zoompx = 20;

                        var newelementheight = elementheight + zoompx;
                        var newelementwidth = elementwidth + zoompx;
                        var newelementmarginheight = elementmarginheight - (zoompx/2);
                        var newelementmarginwidth = elementmarginwidth - (zoompx/2);

                        var newelementimageheight = elementimageheight + zoompx;
                        var newelementimagewidth = elementimagewidth + zoompx;
                        
                        var newelementimagetdheight;
                        var newelementimagetdwidth;
                        if (elementimagetdheight > elementimagetdwidth){
                            newelementimagetdheight = elementimagetdheight + zoompx;
                            newelementimagetdwidth = newelementimagetdheight * (elementimagetdwidth/elementimagetdheight);
                        }else
                        {
                            newelementimagetdwidth = elementimagetdwidth + zoompx;
                            newelementimagetdheight = newelementimagetdwidth * (elementimagetdheight/elementimagetdwidth);
                        };
                        
	        			$(this).animate({
					  		marginTop: newelementmarginheight + "px",
                            marginBottom: newelementmarginheight + "px",
                            marginLeft: newelementmarginwidth + "px",
                            marginRight: newelementmarginwidth + "px",
					  		height: newelementheight + "px",
					  		width: newelementwidth + "px",
	        			}, 100);
	        			$(this).find('.elementimage').animate({
	        				height: newelementimageheight + "px",
	        				width: newelementimagewidth + "px",
	        			},100);
	        			$(this).find('.image').animate({
	        				height: newelementimagetdheight + "px",
	        				width: newelementimagetdwidth + "px",
	        			},100);
					},
					function() {
                    console.log("elementheight - " + elementheight);
						$(this).stop().animate({
					  		marginTop: elementmarginheight + "px",
                            marginBottom: elementmarginheight + "px",
                            marginLeft: elementmarginwidth + "px",
                            marginRight: elementmarginwidth + "px",
					  		height: elementheight + "px",
					  		width: elementwidth + "px",
	        			}, 50);
	        			$(this).find('.elementimage').stop().animate({
	        				height: elementimageheight + "px",
	        				width: elementimagewidth + "px",
	        			},50);
	        			$(this).find('.image').stop().animate({
	        				height: elementimagetdheight + "px",
	        				width: elementimagetdwidth + "px",
	        			},50);
					}
	        	);
        	});
        </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    

    <asp:Repeater ID="RepeaterThumbnails" runat="server">
        <ItemTemplate>
            <div class="elementsurround">
	    		<a href="<%# Eval("pathToSourceImage") %>" title="<%# Eval("filename")%>">
			    	<div class="element">
			    		<table class="elementimage"><tr><td>
			    			<img class="image" src="<%# Eval("pathToThumbnailImage") %>" alt="<%# Eval("filename")%>">
			    		</td></tr></table>
			    		<div class="elementtext">
			    			<p class="text"><%# Eval("filename") %></p>
			    		</div>
			    	</div>
			    </a>
	    	</div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
