﻿//using YouTube.Player;

namespace MovieBuddy.Activities
{
    //[Activity(Label = "TrailerActivity",
    //	ScreenOrientation = ScreenOrientation.Nosensor,
    //	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
    //[MetaData("@string/minVersion", Value = "8")]
    //[MetaData("@string/isLaunchableActivity", Value = "true")]
    //public class TrailerActivity : YouTubeFailureRecoveryActivity, IYouTubePlayerOnFullscreenListener
    //{
    //	private static readonly ScreenOrientation PortraitOrientation =
    //		(int)Build.VERSION.SdkInt < 9 ? ScreenOrientation.Portrait : ScreenOrientation.SensorPortrait;

    //	private LinearLayout baseLayout;
    //	private YouTubePlayerView playerView;
    //	private IYouTubePlayer player;
    //	private Button fullscreenButton;
    //	private CompoundButton checkbox;
    //	private View otherViews;

    //	private bool fullscreen;

    //	protected override void OnCreate(Bundle savedInstanceState)
    //	{
    //		base.OnCreate(savedInstanceState);

    //		//SetContentView(Resource.Layout.fullscreen_demo);

    //		//baseLayout = FindViewById<LinearLayout>(Resource.Id.layout);
    //		//playerView = FindViewById<YouTubePlayerView>(Resource.Id.player);
    //		//fullscreenButton = FindViewById<Button>(Resource.Id.fullscreen_button);
    //		//checkbox = FindViewById<CompoundButton>(Resource.Id.landscape_fullscreen_checkbox);
    //		//otherViews = FindViewById(Resource.Id.other_views);

    //		checkbox.CheckedChange += (sender, e) =>
    //		{
    //			int controlFlags = player.FullscreenControlFlags;
    //			if (e.IsChecked)
    //			{
    //				// If you use the FULLSCREEN_FLAG_ALWAYS_FULLSCREEN_IN_LANDSCAPE, your activity's normal UI
    //				// should never be laid out in landscape mode (since the video will be fullscreen whenever the
    //				// activity is in landscape orientation). Therefore you should set the activity's requested
    //				// orientation to portrait. Typically you would do this in your AndroidManifest.xml, we do it
    //				// programmatically here since this activity demos fullscreen behavior both with and without
    //				// this flag).
    //				RequestedOrientation = PortraitOrientation;
    //				controlFlags |= YouTubePlayer.FullscreenFlagAlwaysFullscreenInLandscape;
    //			}
    //			else
    //			{
    //				RequestedOrientation = ScreenOrientation.Sensor;
    //				controlFlags &= ~YouTubePlayer.FullscreenFlagAlwaysFullscreenInLandscape;
    //			}
    //			player.FullscreenControlFlags = controlFlags;
    //		};

    //		fullscreenButton.Click += (sender, e) =>
    //		{
    //			player.SetFullscreen(!fullscreen);
    //		};

    //		//playerView.Initialize(DeveloperKey.Key, this);

    //		DoLayout();
    //	}

    //	public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
    //	{
    //		Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

    //		base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    //	}



    //	void IYouTubePlayerOnFullscreenListener.OnFullscreen(bool isFullscreen)
    //	{
    //		fullscreen = isFullscreen;
    //		DoLayout();
    //	}

    //	protected override IYouTubePlayerProvider YouTubePlayerProvider => playerView;

    //	public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
    //	{
    //		base.OnConfigurationChanged(newConfig);
    //		DoLayout();
    //	}

    //	private void DoLayout()
    //	{
    //		var playerParams = (LinearLayout.LayoutParams)playerView.LayoutParameters;
    //		if (fullscreen)
    //		{
    //			// When in fullscreen, the visibility of all other views than the player should be set to
    //			// GONE and the player should be laid out across the whole screen.
    //			playerParams.Width = ViewGroup.LayoutParams.MatchParent;
    //			playerParams.Height = ViewGroup.LayoutParams.MatchParent;

    //			otherViews.Visibility = ViewStates.Gone;
    //		}
    //		else
    //		{
    //			// This layout is up to you - this is just a simple example (vertically stacked boxes in
    //			// portrait, horizontally stacked in landscape).
    //			otherViews.Visibility = ViewStates.Visible;
    //			var otherViewsParams = otherViews.LayoutParameters;
    //			if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape)
    //			{
    //				playerParams.Width = otherViewsParams.Width = 0;
    //				playerParams.Height = ViewGroup.LayoutParams.WrapContent;
    //				otherViewsParams.Height = ViewGroup.LayoutParams.MatchParent;
    //				playerParams.Weight = 1;
    //				baseLayout.Orientation = Orientation.Horizontal;
    //			}
    //			else
    //			{
    //				playerParams.Width = otherViewsParams.Width = ViewGroup.LayoutParams.MatchParent;
    //				playerParams.Height = ViewGroup.LayoutParams.WrapContent;
    //				playerParams.Weight = 0;
    //				otherViewsParams.Height = 0;
    //				baseLayout.Orientation = Orientation.Vertical;
    //			}
    //			SetControlsEnabled();
    //		}
    //	}

    //	private void SetControlsEnabled()
    //	{
    //		checkbox.Enabled =
    //			player != null &&
    //			Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait;
    //		fullscreenButton.Enabled = player != null;
    //	}

    //	public override void OnInitializationSuccess(IYouTubePlayerProvider provider, IYouTubePlayer player, bool wasRestored)
    //	{
    //		this.player = player;
    //		SetControlsEnabled();
    //		// Specify that we want to handle fullscreen behavior ourselves.
    //		player.AddFullscreenControlFlag(YouTubePlayer.FullscreenFlagCustomLayout);
    //		player.SetOnFullscreenListener(this);
    //		if (!wasRestored)
    //		{
    //			player.CueVideo("avP5d16wEp0");
    //		}
    //	}
    //}

    //public abstract class YouTubeFailureRecoveryActivity : YouTubeBaseActivity, IYouTubePlayerOnInitializedListener
    //{
    //	public const int RecoveryDialogRequest = 1;

    //	public void OnInitializationFailure(IYouTubePlayerProvider provider, YouTubeInitializationResult errorReason)
    //	{
    //		//if (errorReason.IsUserRecoverableError)
    //		//{
    //		//	errorReason.GetErrorDialog(this, RecoveryDialogRequest).Show();
    //		//}
    //		//else
    //		//{
    //		//	var errorMessage = string.Format(GetString(Resource.String.error_player), errorReason);
    //		//	Toast.MakeText(this, errorMessage, ToastLength.Long).Show();
    //		//}
    //	}

    //	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    //	{
    //		//if (requestCode == RecoveryDialogRequest)
    //		//{
    //		//	// Retry initialization if user performed a recovery action
    //		//	YouTubePlayerProvider.Initialize(DeveloperKey.Key, this);
    //		//}
    //	}

    //	public abstract void OnInitializationSuccess(IYouTubePlayerProvider provider, IYouTubePlayer player, bool wasRestored);

    //	protected abstract IYouTubePlayerProvider YouTubePlayerProvider { get; }
    //}
}